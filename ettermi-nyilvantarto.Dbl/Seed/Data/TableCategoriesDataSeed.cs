using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Seed
{
	public class TableCategoriesDataSeed
	{
		public List<TableCategory> TableCategories { get; } = new List<TableCategory>() {
			new TableCategory() { Id = 1, Name = "Terasz" },
			new TableCategory() { Id = 2, Name = "Családi" },
			new TableCategory() { Id = 3, Name = "Rendezvények" },
			new TableCategory() { Id = 4, Name = "Kicsi" },
			new TableCategory() { Id = 6, Name = "Normál" },
			new TableCategory() { Id = 7, Name = "Nagy" },
			new TableCategory() { Id = 8, Name = "Négy lába van, de nem szék" }
		};
	}
}
