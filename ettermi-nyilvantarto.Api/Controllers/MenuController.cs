using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/menu")]
	[Produces("application/json")]
	public class MenuController : ControllerBase
	{
		private IMenuService MenuService { get; }
		public MenuController(IMenuService menuService)
		{
			this.MenuService = menuService;
		}

		[HttpGet]
		[HttpGet("page/{page}")]
		[Authorize]
		public async Task<IEnumerable<MenuListModel>> GetMenu(int page = 1)
			=> await MenuService.GetMenu(page);

		[HttpPost]
		[Authorize(Roles = "Owner,Chef")]
		public async Task<int> AddMenuItem(MenuAddModel item)
			=> await MenuService.AddMenuItem(item);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner,Chef")]
		public async Task DeleteMenuItem(int id)
			=> await MenuService.DeleteMenuItem(id);

		[HttpGet("categories")]
		[Authorize(Roles = "Owner,Chef")]
		public async Task<IEnumerable<MenuCategoryModel>> ListCategories()
			=> await MenuService.GetCategories();
	}
}
