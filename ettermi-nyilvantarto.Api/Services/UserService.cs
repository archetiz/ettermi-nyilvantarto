﻿using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class UserService : IUserService
	{
		private SignInManager<User> SignInManager { get; }
		//private UserManager<User> UserManager { get; }

		//public UserService(SignInManager<User> signInManager, UserManager<User> userManager)
		public UserService(SignInManager<User> signInManager)
		{
			this.SignInManager = signInManager;
			//this.UserManager = userManager;
		}

		public async Task<LoginResultModel> Login(LoginModel loginModel)
		{
			LoginResultModel loginResult = new LoginResultModel();
			//var user = await UserManager.FindByNameAsync(loginModel.UserName);
			var signInResult = await SignInManager.PasswordSignInAsync(loginModel.UserName, loginModel.Password, false, false);
			if (signInResult.Succeeded)
			{
				loginResult.IsSuccess = true;
				//TODO: JWT token
			}
			else
			{
				loginResult.IsSuccess = false;
			}
			return loginResult;
		}

		public async Task Logout()
		{
			await SignInManager.SignOutAsync();
		}
	}
}