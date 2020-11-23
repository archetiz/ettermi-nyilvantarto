using System.Linq;

namespace ettermi_nyilvantarto.Api
{
	public static class IQueryableExtensions
	{
		public static IQueryable<TType> GetPaged<TType>(this IQueryable<TType> queryable, int pageCount, int pageSize)
		{
			pageCount = (pageCount > 0) ? pageCount : 1;
			return queryable.Skip(pageCount * pageSize).Take(pageSize);
		}
	}
}
