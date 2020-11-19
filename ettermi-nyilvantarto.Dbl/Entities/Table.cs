using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Table
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public int Size { get; set; }
		public bool IsActive { get; set; } = true;  //=status
		public int CategoryId { get; set; }
		public TableCategory Category { get; set; }
	}
}
