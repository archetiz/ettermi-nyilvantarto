using ettermi_nyilvantarto.Dbl.Entities;

namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionPayModel
	{
		public string VoucherCode { get; set; }
		public int? LoyaltyCardNumber { get; set; }
		public bool ShouldRedeemPoints { get; set; } = false;
		public string CustomerName { get; set; }
		public string CustomerTaxNumber { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerEmail { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
	}
}
