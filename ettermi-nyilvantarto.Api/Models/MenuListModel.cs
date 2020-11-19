namespace ettermi_nyilvantarto.Api
{
	public class MenuListModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public int CategoryId { get; set; }
		public string Category { get; set; }
	}
}
