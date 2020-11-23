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
		private RestaurantDbContext DbContext { get; }
		public MenuService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<IEnumerable<MenuListModel>> GetMenu()
			=> await DbContext.MenuItems.Include(mi => mi.Category).OrderBy(mi => mi.Name).Select(mi => new MenuListModel
			{
				Id = mi.Id,
				Name = mi.Name,
				Price = mi.Price,
				CategoryId = mi.CategoryId,
				Category = mi.Category.Name
			}).ToListAsync();

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
			var menuItem = await DbContext.MenuItems.FindAsync(id);
			if (menuItem == null)
				throw new RestaurantNotFoundException("Nem létező étel/ital!");
			menuItem.IsActive = false;
			await DbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<MenuCategoryModel>> GetCategories()
			=> await DbContext.MenuItemCategories.OrderBy(mic => mic.Name).Select(mic => new MenuCategoryModel()
			{
				Id = mic.Id,
				Name = mic.Name
			}).ToListAsync();
	}
}
