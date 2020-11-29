using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IReservationService
	{
		PagedResult<ReservationListModel> GetReservations(int page, List<DateTime> datesFilter);
		Task<AddResult> AddReservation(ReservationAddModel model);
		Task DeleteReservation(int id);
		Task ModifyReservation(int id, ReservationModModel model);
	}
}
