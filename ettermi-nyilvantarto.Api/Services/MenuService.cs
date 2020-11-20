using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class MenuService : IMenuService
	{
		private RestaurantDbContext DbContext { get; set; }
		public MenuService(RestaurantDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IEnumerable<MenuListModel>> GetMenu()
			=> (await DbContext.MenuItems.Include(mi => mi.Category).ToListAsync()).Select(mi => new MenuListModel
			{
				Id = mi.Id,
				Name = mi.Name,
				Price = mi.Price,
				CategoryId = mi.CategoryId,
				Category = mi.Category.Name
			});

		public async Task<int> AddMenuItem(MenuAddModel model)
		{
			var menuItem = DbContext.MenuItems.Add(new MenuItem()
			{
				Name = model.Name,
				Price = model.Price,
				CategoryId = model.CategoryId
			});

			await DbContext.SaveChangesAsync();

			return menuItem.Entity.Id;
		}

		public async Task DeleteMenuItem(int id)
		{
			(await DbContext.MenuItems.FindAsync(id)).IsActive = false;
			await DbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<MenuCategoryModel>> GetCategories()
			=> (await DbContext.MenuItemCategories.ToListAsync()).Select(mic => new MenuCategoryModel()
			{
				Id = mic.Id,
				Name = mic.Name
			});
	}
}
