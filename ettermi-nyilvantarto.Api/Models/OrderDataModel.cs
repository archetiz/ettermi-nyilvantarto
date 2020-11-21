using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public class OrderDataModel
	{
		public int Id { get; set; }
		public int WaiterId { get; set; }
		public string WaiterName { get; set; }
		public int Status { get; set; }
		public List<OrderItemListModel> Items { get; set; }
	}
}
