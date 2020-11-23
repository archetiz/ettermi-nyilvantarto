using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IMenuService
	{
		Task<IEnumerable<MenuListModel>> GetMenu(int page);
		Task<int> AddMenuItem(MenuAddModel model);
		Task DeleteMenuItem(int id);
		Task<IEnumerable<MenuCategoryModel>> GetCategories();
	}
}
