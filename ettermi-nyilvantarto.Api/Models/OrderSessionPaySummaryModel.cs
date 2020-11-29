namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionPaySummaryModel
	{
		public int FullPrice { get; set; }
		public int FinalPrice { get; set; }
		public int? LoyaltyCardNumber { get; set; }
		public int? UsedLoyaltyPoints { get; set; }
		public string VoucherCode { get; set; }
		public int? VoucherTotalDiscountAmount { get; set; }
	}
}
