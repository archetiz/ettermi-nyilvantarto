using System;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Invoice
	{
		public int Id { get; set; }
		public string Path { get; set; }
		public int OrderSessionId { get; set; }
		public OrderSession OrderSession { get; set; }
		public DateTime CreationTime { get; set; }
	}
}
