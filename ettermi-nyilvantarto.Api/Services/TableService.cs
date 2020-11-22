using ettermi_nyilvantarto.Dbl;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ettermi_nyilvantarto.Dbl.Entities;

namespace ettermi_nyilvantarto.Api
{
	public class TableService : ITableService
	{
		private RestaurantDbContext DbContext { get; }
		public TableService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<IEnumerable<TableListModel>> GetTables()
			=> (await DbContext.Tables.Include(t => t.Category).Where(t => t.IsActive).OrderBy(t => t.Code).ToListAsync()).Select(t => new TableListModel()
			{
				Id = t.Id,
				Code = t.Code,
				Size = t.Size,
				CategoryId = t.Category.Id,
				Category = t.Category.Name
			});

		public async Task<int> AddTable(TableAddModel model)
		{
			var table = DbContext.Tables.Add(new Table()
			{
				Code = model.Code,
				Size = model.Size,
				IsActive = true,
				CategoryId = model.CategoryId
			});

			await DbContext.SaveChangesAsync();

			return table.Entity.Id;
		}

		public async Task DeleteTable(int id)
		{
			var table = await DbContext.Tables.FindAsync(id);
			if (table == null)
				throw new RestaurantNotFoundException("Nem létező asztal!");
			table.IsActive = false;
			await DbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<TableCategoryModel>> GetCategories()
			=> (await DbContext.TableCategories.ToListAsync()).Select(tc => new TableCategoryModel()
			{
				Id = tc.Id,
				Name = tc.Name
			});

		public async Task<int?> GetActiveSessionForTable(int id)
			=> (await DbContext.OrderSessions.Where(os => os.TableId == id && os.Status == OrderSessionStatus.Active).SingleOrDefaultAsync())?.Id;
	}
}
