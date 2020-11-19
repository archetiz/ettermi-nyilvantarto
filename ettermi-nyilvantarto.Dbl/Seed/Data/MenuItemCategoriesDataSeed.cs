using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Seed
{
	public class MenuItemCategoriesDataSeed
	{
		public List<MenuItemCategory> MenuItemCategories { get; } = new List<MenuItemCategory>() {
			new MenuItemCategory() { Id = 1, Name = "Előétel" },
			new MenuItemCategory() { Id = 2, Name = "Főétel" },
			new MenuItemCategory() { Id = 3, Name = "Köret" },
			new MenuItemCategory() { Id = 4, Name = "Desszert" },
			new MenuItemCategory() { Id = 5, Name = "Ital" }
		};
	}
}
