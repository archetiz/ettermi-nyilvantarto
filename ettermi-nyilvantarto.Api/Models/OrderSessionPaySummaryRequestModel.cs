namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionPaySummaryRequestModel
	{
		public string VoucherCode { get; set; }
		public int? LoyaltyCardNumber { get; set; }
		public bool ShouldRedeemPoints { get; set; } = false;
	}
}
