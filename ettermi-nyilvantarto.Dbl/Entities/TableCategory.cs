using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class TableCategory
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public List<Table> Tables { get; set; }
	}
}
