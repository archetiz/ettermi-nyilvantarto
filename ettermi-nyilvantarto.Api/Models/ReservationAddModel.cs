using System;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationAddModel
	{
		public int TableId { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhone { get; set; }
	}
}
