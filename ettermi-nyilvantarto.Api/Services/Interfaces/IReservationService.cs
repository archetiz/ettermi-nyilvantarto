using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IReservationService
	{
		Task<PagedResult<ReservationListModel>> GetReservations(int page);
		Task<AddResult> AddReservation(ReservationAddModel model);
		Task DeleteReservation(int id);
		Task ModifyReservation(int id, ReservationModModel model);
	}
}
