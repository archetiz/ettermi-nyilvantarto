using Newtonsoft.Json;

namespace ettermi_nyilvantarto.Dbl.Configurations
{
	public class InvoiceConfiguration
	{
		public string Directory { get; set; }
		public string FontFamily { get; set; }
		public int FontSize { get; set; }
		public int Margin { get; set; }

		[JsonProperty("Restaurant")]
		public InvoiceRestaurantConfiguration Restaurant { get; set; }
	}
}
