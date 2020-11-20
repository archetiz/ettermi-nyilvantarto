using System;

namespace ettermi_nyilvantarto.Api
{
	public class ReservationModModel
	{
		public int? TableId { get; set; }
		public DateTime? TimeFrom { get; set; }
		public DateTime? TimeTo { get; set; }
	}
}
