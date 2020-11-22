namespace ettermi_nyilvantarto.Api
{
	public class OrderItemAddModel
	{
		public int OrderId { get; set; }
		public int MenuItemId { get; set; }
		public int Quantity { get; set; }
		public string Comment { get; set; }
	}
}
