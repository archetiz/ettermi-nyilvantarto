using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ITableService
	{
		Task<IEnumerable<TableListModel>> GetTables();
		Task<int> AddTable(TableAddModel model);
		Task DeleteTable(int id);
		Task<IEnumerable<TableCategoryModel>> GetCategories();
		Task<int?> GetActiveSessionForTable(int id);
		Task<IEnumerable<TableFreeModel>> GetFreeTables(TableFreeFilterModel filter);
	}
}
