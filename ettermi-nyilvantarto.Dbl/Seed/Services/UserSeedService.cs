using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Dbl.Seed
{
	public class UserSeedService : IUserSeedService
	{
		private readonly UserManager<User> userManager;

		public UserSeedService(UserManager<User> userManager)
		{
			this.userManager = userManager;
		}

		public async Task SeedUserAsync()
		{
			if (!(await userManager.GetUsersInRoleAsync(Roles.Owner)).Any())
			{
				var user = new User
				{
					Email = "archetiz@outlook.com",		//NOTE: temporarily
					Name = "The owner",
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = "owner"
				};

				var createResult = await userManager.CreateAsync(user, "asd.Qwe123");
				var addToResult = await userManager.AddToRoleAsync(user, Roles.Owner);

				if (!createResult.Succeeded || !addToResult.Succeeded)
					throw new ApplicationException("Seeding error: Couldn't create owner!");
			}
		}
	}
}
