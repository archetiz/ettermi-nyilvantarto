using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IUserService
	{
		Task<LoginResultModel> Login(LoginModel loginModel);
		Task Logout();
		Task<User> GetCurrentUser();
		Task<string> GetCurrentUserRole();
		Task<IEnumerable<UserListModel>> GetUsers();
		Task<int> AddUser(UserAddModel model);
		Task DeleteUser(int id);
		Task ChangePassword(UserPasswordChangeModel model);
		Task SetPassword(int userId, UserPasswordSetModel model);
	}
}
