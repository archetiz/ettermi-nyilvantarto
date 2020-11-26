using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public static class ListExtensions
	{
		public static PagedResult<T> GetPagedResult<T>(this List<T> list, int pageCount, int pageSize, int totalPages)
			=>  new PagedResult<T>()
				{
					CurrentPage = pageCount,
					PageSize = pageSize,
					TotalPages = totalPages,
					Elements = list
				};
	}
}
