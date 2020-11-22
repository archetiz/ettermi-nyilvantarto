using ettermi_nyilvantarto.Dbl.Entities;

namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionListModel
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public int? CustomerId { get; set; }
		public int? VoucherId { get; set; }
		public int? InvoiceId { get; set; }
		public OrderSessionStatus Status { get; set; }
	}
}
