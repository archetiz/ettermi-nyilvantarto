using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public class PagedResult<T>
	{
		public int CurrentPage { get; set; }
		public int PageSize { get; set; }
		public int TotalPages { get; set; }
		public IEnumerable<T> Elements { get; set; }
	}
}
