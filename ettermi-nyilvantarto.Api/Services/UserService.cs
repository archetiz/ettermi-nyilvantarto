﻿using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class UserService : IUserService
	{
		private SignInManager<User> SignInManager { get; }
		private UserManager<User> UserManager { get; }
		private IHttpContextAccessor HttpContextAccessor { get; }
		private RestaurantDbContext DbContext { get; }

		public UserService(SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, RestaurantDbContext dbContext)
		{
			this.SignInManager = signInManager;
			this.UserManager = userManager;
			this.HttpContextAccessor = httpContextAccessor;
			this.DbContext = dbContext;
		}

		public async Task<LoginResultModel> Login(LoginModel loginModel)
		{
			LoginResultModel loginResult = new LoginResultModel();
			var user = await UserManager.FindByNameAsync(loginModel.UserName);

			if (user == null || !user.IsActive)
				throw new RestaurantNotFoundException("Hibás vagy nem létező felhaszáló!");

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

		public async Task<User> GetCurrentUser()
			=> await UserManager.GetUserAsync(HttpContextAccessor.HttpContext.User);

		public async Task<string> GetCurrentUserRole()
			=> (await UserManager.GetRolesAsync(await GetCurrentUser()))[0];

		public async Task<IEnumerable<UserListModel>> GetUsers()
		{
			var users = await DbContext.Users.Where(u => u.IsActive).OrderBy(u => u.UserName).ToListAsync();

			var userList = new List<UserListModel>();

			foreach(var user in users)
			{
				var userListElement = new UserListModel()
				{
					Id = user.Id,
					Email = user.Email,
					Name = user.Name,
					UserName = user.UserName,
					AccountType = (await UserManager.GetRolesAsync(user))[0]
				};
				userList.Add(userListElement);
			}
			return userList;
		}

		public async Task<int> AddUser(UserAddModel model)
		{
			if (model.AccountType == Roles.Owner)
				throw new RestaurantBadRequestException("Csak egyetlen tulajdonos felhasználó létezhet!");

			if (model.AccountType != Roles.Chef && model.AccountType != Roles.Waiter)
				throw new RestaurantBadRequestException("Hibás felhasználói szerepkör!");

			var existingUser = await UserManager.FindByNameAsync(model.UserName);
			if (existingUser != null)
				throw new RestaurantBadRequestException("A megadott felhasználónév foglalt!");

			var user = new User
			{
				Email = model.Email,
				Name = model.Name,
				UserName = model.UserName,
				SecurityStamp = Guid.NewGuid().ToString(),
				IsActive = true
			};

			var createResult = await UserManager.CreateAsync(user, model.Password);
			var addToResult = await UserManager.AddToRoleAsync(user, model.AccountType);

			if (!createResult.Succeeded || !addToResult.Succeeded)
				throw new RestaurantInternalServerErrorException("Sikertelen felhasználó létrehozás!");

			return user.Id;
		}

		public async Task DeleteUser(int id)
		{
			var user = await DbContext.Users.FindAsync(id);

			if (user == null)
				throw new RestaurantNotFoundException("Nem létező felhasználó!");

			user.IsActive = false;
			await UserManager.SetLockoutEnabledAsync(user, true);
			await UserManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);

			await DbContext.SaveChangesAsync();
		}
	}
}
