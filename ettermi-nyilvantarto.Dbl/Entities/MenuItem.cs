namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class MenuItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		//public int CategoryId { get; set; }	//????
		public bool IsActive { get; set; } = true;	//=status
	}
}
