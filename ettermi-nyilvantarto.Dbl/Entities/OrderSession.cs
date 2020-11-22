using System;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class OrderSession
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public Table Table { get; set; }
		public int? CustomerId { get; set; }
		public Customer Customer { get; set; }
		public int? VoucherId { get; set; }
		public Voucher Voucher { get; set; }
		public int? InvoiceId { get; set; }
		public Invoice Invoice { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime ClosedAt { get; set; }
		public OrderSessionStatus Status { get; set; }
		public List<Order> Orders { get; set; }
	}
}
