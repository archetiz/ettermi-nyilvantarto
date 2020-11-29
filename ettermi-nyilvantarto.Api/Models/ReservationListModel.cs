using System;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationListModel
	{
		public int Id { get; set; }
		public int TableId { get; set; }
		public string TableCode { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
		public string CustomerAddress { get; set; }
	}
}
