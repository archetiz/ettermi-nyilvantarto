using ettermi_nyilvantarto.Dbl.Entities;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IUserService
	{
		Task<LoginResultModel> Login(LoginModel loginModel);
		Task Logout();
		Task<User> GetCurrentUser();
		Task<string> GetCurrentUserRole();
		Task<PagedResult<UserListModel>> GetUsers(int page);
		Task<UserDataModel> GetCurrentUserData();
		Task<int> AddUser(UserAddModel model);
		Task DeleteUser(int id);
		Task ChangePassword(UserPasswordChangeModel model);
		Task SetPassword(int userId, UserPasswordSetModel model);
	}
}
