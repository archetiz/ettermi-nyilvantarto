using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ITableService
	{
		Task<IEnumerable<TableListModel>> GetTables();
		Task<AddResult> AddTable(TableAddModel model);
		Task DeleteTable(int id);
		Task<IEnumerable<TableCategoryModel>> GetCategories();
		Task<TableSessionModel> GetActiveSessionForTable(int id);
		IEnumerable<TableFreeModel> GetFreeTables(TableFreeFilterModel filter);
		Task<bool> IsTableAvailable(int tableId, DateTime timeFrom, DateTime timeTo, int? excludeReservationId = null);
	}
}
