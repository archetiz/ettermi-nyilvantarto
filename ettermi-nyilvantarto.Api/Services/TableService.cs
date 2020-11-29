using ettermi_nyilvantarto.Dbl;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ettermi_nyilvantarto.Dbl.Entities;
using System;

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
			=> await DbContext.Tables.Include(t => t.Category).Where(t => t.IsActive).OrderBy(t => t.Code).Select(t => new TableListModel()
			{
				Id = t.Id,
				Code = t.Code,
				Size = t.Size,
				CategoryId = t.CategoryId,
				Category = ((t.Category != null) ? t.Category.Name : "")
			}).ToListAsync();

		public async Task<AddResult> AddTable(TableAddModel model)
		{
			if (string.IsNullOrEmpty(model.Code))
				throw new RestaurantBadRequestException("Az asztal kódja nem lehet üres!");

			if (model.Size <= 0)
				throw new RestaurantBadRequestException("Az asztal méretének pozitív számnak kell lennie!");

			var existingTable = await DbContext.Tables.Where(t => t.Code == model.Code && t.IsActive).SingleOrDefaultAsync();

			if (existingTable != null)
				throw new RestaurantBadRequestException("Már létezik asztal a megadott kóddal!");

			if (model.CategoryId != null)
			{
				var category = await DbContext.TableCategories.FindAsync(model.CategoryId);
				if (category == null)
					throw new RestaurantNotFoundException("A megadott kategória nem létezik!");
			}

			var table = DbContext.Tables.Add(new Table()
			{
				Code = model.Code,
				Size = model.Size,
				IsActive = true,
				CategoryId = model.CategoryId
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(table.Entity.Id);
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
			=> await DbContext.TableCategories.OrderBy(t => t.Name).Select(tc => new TableCategoryModel()
			{
				Id = tc.Id,
				Name = tc.Name
			}).ToListAsync();

		public async Task<TableSessionModel> GetActiveSessionForTable(int id)
			=> new TableSessionModel() { ActiveSessionId = (await DbContext.OrderSessions.Where(os => os.TableId == id && os.Status == OrderSessionStatus.Active).SingleOrDefaultAsync())?.Id };

		public IEnumerable<TableFreeModel> GetFreeTables(TableFreeFilterModel filter)
			=> DbContext.Tables
								.Include(t => t.Reservations)
								.Include(t => t.Category)
								.AsEnumerable()
								.Where(t => CheckTable(t, filter))
								.Select(t => new TableFreeModel()
								{
									Id = t.Id,
									Code = t.Code,
									Size = t.Size,
									CategoryId = t.CategoryId,
									CategoryName = t.Category?.Name
								}).ToList();

		private bool CheckTable(Table table, TableFreeFilterModel filter, int? excludeReservationId = null)
		{
			if (table.Size < (filter.MinSize ?? 0))
				return false;

			var overlappingReservations = table.Reservations.Where(r => r.IsActive && r.TimeFrom <= filter.TimeTo && r.TimeTo >= filter.TimeFrom && (excludeReservationId == null || r.Id != excludeReservationId));
			return (overlappingReservations.Count() == 0);
		}

		public async Task<bool> IsTableAvailable(int tableId, DateTime timeFrom, DateTime timeTo, int? excludeReservationId = null)
		{
			var table = await DbContext.Tables.Include(t => t.Reservations).SingleOrDefaultAsync(t => t.Id == tableId);

			return CheckTable(table, new TableFreeFilterModel()
			{
				TimeFrom = timeFrom,
				TimeTo = timeTo
			}, excludeReservationId);
		}
	}
}
