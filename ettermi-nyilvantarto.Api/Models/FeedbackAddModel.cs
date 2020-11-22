namespace ettermi_nyilvantarto.Api
{
	public class FeedbackAddModel
	{
		public int OrderSessionId { get; set; }
		public int Rating { get; set; }
		public string Comment { get; set; }
	}
}
