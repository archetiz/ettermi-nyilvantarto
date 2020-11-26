using ettermi_nyilvantarto.Dbl.Entities;
using System;

namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionListModel
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public string TableCode { get; set; }
		public int? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public int? VoucherId { get; set; }
		public int? InvoiceId { get; set; }
		public string Status { get; set; }
		public DateTime OpenedAt { get; set; }
		public DateTime? ClosedAt { get; set; }
	}
}
