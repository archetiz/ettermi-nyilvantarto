using System;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionDataModel
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public string TableCode { get; set; }
		public int? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerAddress { get; set; }
		public int? VoucherId { get; set; }
		public string VoucherCode { get; set; }
		public int? VoucherDiscountPercentage { get; set; }
		public int? VoucherDiscountAmount { get; set; }
		public int? InvoiceId { get; set; }
		public int Status { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
		public List<OrderListModel> Orders { get; set; }
	}
}
