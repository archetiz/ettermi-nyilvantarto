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
		private RestaurantDbContext DbContext { get; set; }
		public TableService(RestaurantDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IEnumerable<TableListModel>> GetTables()
			=> (await DbContext.Tables.ToListAsync()).Select(t => new TableListModel()
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
			(await DbContext.Tables.FindAsync(id)).IsActive = false;
		}

		public async Task<IEnumerable<TableCategoryModel>> GetCategories()
			=> (await DbContext.TableCategories.ToListAsync()).Select(tc => new TableCategoryModel()
			{
				Id = tc.Id,
				Name = tc.Name
			});
	}
}
