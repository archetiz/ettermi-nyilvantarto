using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationService : IReservationService
	{
		private RestaurantDbContext DbContext { get; }
		public ReservationService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<IEnumerable<ReservationListModel>> GetReservations()
			=> (await DbContext.Reservations
							.Include(r => r.Customer)
							.Where(r => r.IsActive)
							.OrderBy(r => r.TimeFrom).ThenBy(r => r.TableId)
							.ToListAsync())
								.Select(r => new ReservationListModel
								{
									Id = r.Id,
									TableId = r.TableId,
									TimeFrom = r.TimeFrom,
									TimeTo = r.TimeTo,
									CustomerName = r.Customer.Name,
									CustomerPhone = r.Customer.PhoneNumber,
									CustomerAddress = r.Customer.Address
								});

		public async Task<int> AddReservation(ReservationAddModel model)
		{
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

			reservation.TableId = model.TableId ?? reservation.TableId;
			reservation.TimeFrom = model.TimeFrom ?? reservation.TimeFrom;
			reservation.TimeTo = model.TimeTo ?? reservation.TimeTo;

			await DbContext.SaveChangesAsync();
		}
	}
}
