namespace ettermi_nyilvantarto.Api
{
	public class LoginResultModel
	{
		public bool IsSuccess { get; set; }
		public string Token { get; set; }
		public string RefreshToken { get; set; }
	}
}
