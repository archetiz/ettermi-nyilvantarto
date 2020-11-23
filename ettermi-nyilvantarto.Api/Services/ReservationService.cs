using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationService : IReservationService
	{
		private RestaurantDbContext DbContext { get; }
		private ITableService TableService { get; }
		private PagingConfiguration PagingConfig { get; }
		public ReservationService(RestaurantDbContext dbContext, ITableService tableService, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.TableService = tableService;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<IEnumerable<ReservationListModel>> GetReservations(int page)
			=> await DbContext.Reservations
							.Include(r => r.Customer)
							.Where(r => r.IsActive)
							.OrderBy(r => r.TimeFrom).ThenBy(r => r.TableId)
							.GetPaged(page, PagingConfig.PageSize)
							.Select(r => new ReservationListModel
							{
								Id = r.Id,
								TableId = r.TableId,
								TimeFrom = r.TimeFrom,
								TimeTo = r.TimeTo,
								CustomerName = r.Customer.Name,
								CustomerPhone = r.Customer.PhoneNumber,
								CustomerAddress = r.Customer.Address
							}).ToListAsync();

		public async Task<int> AddReservation(ReservationAddModel model)
		{
			if (!(await TableService.IsTableAvailable(model.TableId, model.TimeFrom, model.TimeTo)))
				throw new RestaurantBadRequestException("A foglalás nem teljesíthető: a megadott asztal foglalt a választott időintervallumban!");

			var reservation = DbContext.Reservations.Add(new Reservation()
			{
				TableId = model.TableId,
				TimeFrom = model.TimeFrom,
				TimeTo = model.TimeTo,
				CustomerId = model.CustomerId
			});

			await DbContext.SaveChangesAsync();

			return reservation.Entity.Id;
		}

		public async Task DeleteReservation(int id)
		{
			var reservation = await DbContext.Reservations.FindAsync(id);
			if (reservation == null)
				throw new RestaurantNotFoundException("Nem létező foglalás!");
			reservation.IsActive = false;
			await DbContext.SaveChangesAsync();
		}

		public async Task ModifyReservation(int id, ReservationModModel model)
		{
			var reservation = await DbContext.Reservations.FindAsync(id);

			if (reservation == null)
				throw new RestaurantNotFoundException("Nem létező foglalás!");

			var tableId = model.TableId ?? reservation.TableId;
			var timeFrom = model.TimeFrom ?? reservation.TimeFrom;
			var timeTo = model.TimeTo ?? reservation.TimeTo;

			if (!(await TableService.IsTableAvailable(tableId, timeFrom, timeTo)))
				throw new RestaurantBadRequestException("A megadott asztal foglalt a választott időintervallumban!");

			reservation.TableId = tableId;
			reservation.TimeFrom = timeFrom;
			reservation.TimeTo = timeTo;

			await DbContext.SaveChangesAsync();
		}
	}
}
