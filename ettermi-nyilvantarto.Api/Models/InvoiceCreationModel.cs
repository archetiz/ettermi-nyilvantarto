using ettermi_nyilvantarto.Dbl.Entities;

namespace ettermi_nyilvantarto.Api
{
	public class InvoiceCreationModel
	{
		public int FullPrice { get; set; }
		public int FinalPrice { get; set; }
		public string VoucherCode { get; set; }
		public int? VoucherDiscountAmount { get; set; }
		public int RedeemedLoyaltyPoints { get; set; }
		public string CustomerName { get; set; }
		public string CustomerTaxNumber { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerEmail { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public OrderSession OrderSession { get; set; }
	}
}
