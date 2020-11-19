using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Order
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public Table Table { get; set; }
		public int WaiterUserId { get; set; }
		public User Waiter { get; set; }
		public int? CustomerId { get; set; }
		public Customer Customer { get; set; }
		public int? VoucherId { get; set; }
		public Voucher Voucher { get; set; }
		public OrderStatus Status { get; set; }
		public string InvoicePath { get; set; }
		public List<OrderItem> Items { get; set; }
	}
}
