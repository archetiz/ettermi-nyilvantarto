using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/table")]
	[Produces("application/json")]
	public class TableController : ControllerBase
	{
		private ITableService TableService { get; set; }
		public TableController(ITableService tableService)
		{
			this.TableService = tableService;
		}

		[HttpGet]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<IEnumerable<TableListModel>> ListTables()
			=> await TableService.GetTables();

		[HttpPost]
		[Authorize(Roles = "Owner")]
		public async Task<int> AddTable(TableAddModel table)
			=> await TableService.AddTable(table);

		[HttpDelete]
		[Authorize(Roles = "Owner")]
		public async Task DeleteTable(int id)
			=> await TableService.DeleteTable(id);

		[HttpGet("categories")]
		[Authorize(Roles = "Owner")]
		public async Task<IEnumerable<TableCategoryModel>> ListCategories()
			=> await TableService.GetCategories();
	}
}
