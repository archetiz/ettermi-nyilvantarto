namespace ettermi_nyilvantarto.Api
{
	public class TableListModel
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public int Size { get; set; }
		public int? CategoryId { get; set; }
		public string Category { get; set; }
	}
}
