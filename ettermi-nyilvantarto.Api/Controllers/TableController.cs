﻿using Microsoft.AspNetCore.Authorization;
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
		private ITableService TableService { get; }
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
		public async Task<AddResult> AddTable(TableAddModel table)
			=> await TableService.AddTable(table);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner")]
		public async Task DeleteTable(int id)
			=> await TableService.DeleteTable(id);

		[HttpGet("categories")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<IEnumerable<TableCategoryModel>> ListCategories()
			=> await TableService.GetCategories();

		[HttpGet("{id}/session")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<TableSessionModel> GetTableSessionId(int id)
			=> await TableService.GetActiveSessionForTable(id);

		[HttpGet("free")]
		[Authorize(Roles = "Owner,Waiter")]
		public IEnumerable<TableFreeModel> GetFreeTables([FromQuery] TableFreeFilterModel filter)
				=> TableService.GetFreeTables(filter);
	}
}
