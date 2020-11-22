using System;

namespace ettermi_nyilvantarto.Api
{
	public class TableFreeFilterModel
	{
		public int? MinSize { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
	}
}
