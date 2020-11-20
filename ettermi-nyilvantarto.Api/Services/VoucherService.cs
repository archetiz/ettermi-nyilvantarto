﻿using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class VoucherService : IVoucherService
	{
		private RestaurantDbContext DbContext { get; set; }
		public VoucherService(RestaurantDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IEnumerable<VoucherListModel>> GetVouchers()
			=> (await DbContext.Vouchers.Where(v => v.IsActive).ToListAsync()).Select(v => new VoucherListModel
			{
				Id = v.Id,
				Code = v.Code,
				DiscountThreshold = v.DiscountThreshold,
				DiscountPercentage = v.DiscountPercentage,
				DiscountAmount = v.DiscountAmount,
				ActiveFrom = v.ActiveFrom,
				ActiveTo = v.ActiveTo
			});

		public async Task<int> AddVoucher(VoucherAddModel model)
		{
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

			return voucher.Entity.Id;
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