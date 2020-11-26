using System;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public class OrderDataModel
	{
		public int Id { get; set; }
		public int OrderSessionId { get; set; }
		public int WaiterId { get; set; }
		public string WaiterName { get; set; }
		public int? TableId { get; set; }
		public string TableCode { get; set; }
		public int? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerAddress { get; set; }
		public string Status { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public int Price { get; set; }
		public List<OrderItemListModel> Items { get; set; }
	}
}
