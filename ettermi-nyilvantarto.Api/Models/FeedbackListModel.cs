using System;

namespace ettermi_nyilvantarto.Api
{
	public class FeedbackListModel
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public int Rating { get; set; }
		public string Comment { get; set; }
		public DateTime Date { get; set; }
	}
}
