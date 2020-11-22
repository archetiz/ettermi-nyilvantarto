using System;

namespace ettermi_nyilvantarto.Api
{
	public class OrderListModel
	{
		public int Id { get; set; }
		public int WaiterId { get; set; }
		public int Status { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
	}
}
