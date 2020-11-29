using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationGroupingByTable
	{
		public int TableId { get; set; }
		public IEnumerable<ReservationListModel> Reservations { get; set; }
	}
}
