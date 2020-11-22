using System;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Order
	{
		public int Id { get; set; }
		public int WaiterUserId { get; set; }
		public User Waiter { get; set; }
		public OrderStatus Status { get; set; }
		public int OrderSessionId { get; set; }
		public OrderSession OrderSession { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime ClosedAt { get; set; }
		public List<OrderItem> Items { get; set; }
	}
}
