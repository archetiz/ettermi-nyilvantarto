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
	}
}
