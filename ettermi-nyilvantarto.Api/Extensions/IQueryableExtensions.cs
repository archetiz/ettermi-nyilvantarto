using System;
using System.Linq;

namespace ettermi_nyilvantarto.Api
{
	public static class IQueryableExtensions
	{
		public static IQueryable<TType> GetPaged<TType>(this IQueryable<TType> queryable, int pageCount, int pageSize, out int totalPageCount)
		{
			pageCount = (pageCount > 0) ? pageCount : 1;
			totalPageCount = (int)Math.Ceiling((double)queryable.Count() / pageSize);
			return queryable.Skip((pageCount - 1) * pageSize).Take(pageSize);
		}
	}
}
