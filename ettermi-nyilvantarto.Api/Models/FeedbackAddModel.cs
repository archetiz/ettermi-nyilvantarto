namespace ettermi_nyilvantarto.Api
{
	public class FeedbackAddModel
	{
		public int OrderId { get; set; }
		public int Rating { get; set; }
		public string Comment { get; set; }
	}
}
