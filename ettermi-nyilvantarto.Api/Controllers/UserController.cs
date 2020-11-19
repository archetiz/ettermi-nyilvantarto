using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/user")]
	[Produces("application/json")]
	public class UserController : ControllerBase
	{
		private IUserService UserService { get; }

		public UserController(IUserService userService)
		{
			this.UserService = userService;
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<LoginResultModel> Login(LoginModel loginModel)
			=> await UserService.Login(loginModel);

		[HttpPost("logout")]
		public async Task Logout()
			=> await UserService.Logout();
	}
}
