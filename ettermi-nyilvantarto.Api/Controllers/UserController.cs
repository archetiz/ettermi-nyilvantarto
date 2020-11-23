using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

		[HttpGet]
		[Authorize(Roles = "Owner")]
		public async Task<IEnumerable<UserListModel>> ListUsers()
			=> await UserService.GetUsers();

		[HttpPost]
		[Authorize(Roles = "Owner")]
		public async Task<int> AddUser(UserAddModel user)
			=> await UserService.AddUser(user);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner")]
		public async Task DeleteUser(int id)
			=> await UserService.DeleteUser(id);

		[HttpPut("password")]
		[Authorize]
		public async Task ChangePassword(UserPasswordChangeModel passwordModel)
			=> await UserService.ChangePassword(passwordModel);

		[HttpPut("{userId}/password")]
		[Authorize(Roles = "Owner")]
		public async Task SetUserPassword(int userId, UserPasswordSetModel passwordModel)
			=> await UserService.SetPassword(userId, passwordModel);
	}
}
