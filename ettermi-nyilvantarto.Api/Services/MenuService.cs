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
			=> await DbContext.MenuItems.Include(mi => mi.Category)
										.Where(mi => mi.IsActive)
										.OrderBy(mi => mi.Name)
										.Select(mi => new MenuListModel
										{
											Id = mi.Id,
											Name = mi.Name,
											Price = mi.Price,
											CategoryId = mi.CategoryId,
											Category = mi.Category.Name
										}).ToListAsync();

		public async Task<AddResult> AddMenuItem(MenuAddModel model)
		{
			if (model.Price < 1)
				throw new RestaurantBadRequestException("Az ár nem lehet kisebb 1-nél!");

			var existingMenuItem = await DbContext.MenuItems.Where(mi => mi.Name == model.Name && mi.IsActive).SingleOrDefaultAsync();

			if (existingMenuItem != null)
				throw new RestaurantBadRequestException("Már létezik étel/ital ezzel a névvel!");

			var category = await DbContext.MenuItemCategories.FindAsync(model.CategoryId);

			if (category == null)
				throw new RestaurantNotFoundException("A megadott kategória nem létezik!");

			var menuItem = DbContext.MenuItems.Add(new MenuItem()
			{
				Name = model.Name,
				Price = model.Price,
				CategoryId = model.CategoryId
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(menuItem.Entity.Id);
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
