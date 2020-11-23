using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/reservation")]
	[Produces("application/json")]
	[Authorize(Roles = "Owner,Waiter")]
	public class ReservationController : ControllerBase
	{
		private IReservationService ReservationService { get; }
		public ReservationController(IReservationService reservationService)
		{
			this.ReservationService = reservationService;
		}

		[HttpGet]
		[HttpGet("page/{page}")]
		public async Task<IEnumerable<ReservationListModel>> GetReservations(int page = 1)
			=> await ReservationService.GetReservations(page);

		[HttpPost]
		public async Task<int> AddReservation(ReservationAddModel reservation)
			=> await ReservationService.AddReservation(reservation);

		[HttpDelete("{id}")]
		public async Task DeleteReservation(int id)
			=> await ReservationService.DeleteReservation(id);

		[HttpPut("{id}")]
		public async Task ModifyReservation(int id, ReservationModModel reservationModel)
			=> await ReservationService.ModifyReservation(id, reservationModel);
	}
}
