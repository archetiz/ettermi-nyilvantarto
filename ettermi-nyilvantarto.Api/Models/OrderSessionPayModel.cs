namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionPayModel
	{
		public string VoucherCode { get; set; }
		public int? LoyaltyCardNumber { get; set; }
		public bool ShouldRedeemPoints { get; set; } = false;
	}
}
