using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class MenuItemCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<MenuItem> MenuItems { get; set; }
	}
}
