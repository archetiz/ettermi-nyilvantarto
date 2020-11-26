using System;

namespace ettermi_nyilvantarto.Api
{
	public class OrderListModel
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public string TableCode { get; set; }
		public int WaiterId { get; set; }
		public string WaiterName { get; set; }
		public string Status { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
	}
}
