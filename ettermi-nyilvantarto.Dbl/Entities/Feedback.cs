using System;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Feedback
	{
		public int Id { get; set; }
		public int OrderSessionId { get; set; }
		public OrderSession OrderSession { get; set; }
		public int Rating { get; set; } //0-5
		public string Comment { get; set; }
		public DateTime Date { get; set; }
	}
}
