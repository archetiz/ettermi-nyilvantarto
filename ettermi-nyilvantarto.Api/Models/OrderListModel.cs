namespace ettermi_nyilvantarto.Api
{
	public class OrderListModel
	{
		public int Id { get; set; }
		public int? TableId { get; set; }
		public int WaiterId { get; set; }
		public int Status { get; set; }
		public int? CustomerId { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerAddress { get; set; }
	}
}
