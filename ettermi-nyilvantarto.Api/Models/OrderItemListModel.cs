namespace ettermi_nyilvantarto.Api
{
	public class OrderItemListModel
	{
		public int OrderItemId { get; set; }
		public int MenuItemId { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public int Price { get; set; }
		public string Comment { get; set; }
		public int MenuItemCategoryId { get; set; }
		public string MenuItemCategoryName { get; set; }

	}
}
