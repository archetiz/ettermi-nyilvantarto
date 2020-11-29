using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class VoucherService : IVoucherService
	{
		private RestaurantDbContext DbContext { get; }
		private PagingConfiguration PagingConfig { get; }
		public VoucherService(RestaurantDbContext dbContext, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<PagedResult<VoucherListModel>> GetVouchers(int page)
			=> (await DbContext.Vouchers.Where(v => v.IsActive && v.ActiveFrom <= DateTime.Now && v.ActiveTo > DateTime.Now)
										.OrderByDescending(v => v.ActiveTo)
										.GetPaged(page, PagingConfig.PageSize, out int totalPages)
										.Select(v => new VoucherListModel
										{
											Id = v.Id,
											Code = v.Code,
											DiscountThreshold = v.DiscountThreshold,
											DiscountPercentage = v.DiscountPercentage,
											DiscountAmount = v.DiscountAmount,
											ActiveFrom = v.ActiveFrom,
											ActiveTo = v.ActiveTo
										}).ToListAsync()).GetPagedResult(page, PagingConfig.PageSize, totalPages);

		public async Task<AddResult> AddVoucher(VoucherAddModel model)
		{
			if (string.IsNullOrEmpty(model.Code))
				throw new RestaurantBadRequestException("A kupon kódja nem lehet üres!");

			if (model.ActiveTo < DateTime.Now)
				throw new RestaurantBadRequestException("A végdátumnak az aktuális időpontnál későbbre kell esnie!");

			if (model.ActiveFrom >= model.ActiveTo)
				throw new RestaurantBadRequestException("A végdátumnak a kezdő dátumnál későbbre kell esnie!");

			if ((model.DiscountAmount ?? 1) <= 0 && (model.DiscountPercentage ?? 1) <= 0)
				throw new RestaurantBadRequestException("A kedvezmény értékének pozitív számnak kell lennie!");

			var existingVoucher = await DbContext.Vouchers
													.Where(v => v.Code == model.Code && v.IsActive && v.ActiveFrom <= DateTime.Now && v.ActiveTo > DateTime.Now)
													.SingleOrDefaultAsync();

			if (existingVoucher != null)
				throw new RestaurantBadRequestException("Már létezik kupon ezzel a kóddal!");

			var voucher = DbContext.Vouchers.Add(new Voucher()
			{
				Code = model.Code,
				DiscountThreshold = model.DiscountThreshold ?? 0,
				DiscountPercentage = model.DiscountPercentage,
				DiscountAmount = model.DiscountAmount,
				ActiveFrom = model.ActiveFrom,
				ActiveTo = model.ActiveTo
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(voucher.Entity.Id);
		}

		public async Task ModifyVoucher(int id, VoucherModModel model)
		{
			var voucher = await DbContext.Vouchers.FindAsync(id);

			if (voucher == null)
				throw new RestaurantNotFoundException("Nem létező kupon!");

			if (!(voucher.IsActive && voucher.ActiveFrom <= DateTime.Now && voucher.ActiveTo > DateTime.Now))
				throw new RestaurantBadRequestException("Lejárt kupont nem lehet módosítani!");

			if (model.ActiveTo < DateTime.Now)
				throw new RestaurantBadRequestException("A végdátumnak az aktuális időpontnál későbbre kell esnie!");

			voucher.ActiveTo = model.ActiveTo;

			await DbContext.SaveChangesAsync();
		}

		public async Task DeleteVoucher(int id)
		{
			var voucher = await DbContext.Vouchers.FindAsync(id);
			if (voucher == null)
				throw new RestaurantNotFoundException("Nem létező kupon!");
			voucher.IsActive = false;
			await DbContext.SaveChangesAsync();
		}
	}
}
