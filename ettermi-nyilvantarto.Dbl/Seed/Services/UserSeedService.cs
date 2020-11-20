using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Dbl.Seed
{
	public class UserSeedService : IUserSeedService
	{
		private readonly UserManager<User> userManager;
		private readonly OwnerConfiguration ownerConfiguration;

		public UserSeedService(UserManager<User> userManager, IOptions<OwnerConfiguration> config)
		{
			this.userManager = userManager;
			this.ownerConfiguration = config.Value;
		}

		public async Task SeedUserAsync()
		{
			if (!(await userManager.GetUsersInRoleAsync(Roles.Owner)).Any())
			{
				var user = new User
				{
					Email = ownerConfiguration.Email,
					Name = ownerConfiguration.Name,
					SecurityStamp = Guid.NewGuid().ToString(),
					UserName = ownerConfiguration.UserName
				};

				var createResult = await userManager.CreateAsync(user, ownerConfiguration.Password);
				var addToResult = await userManager.AddToRoleAsync(user, Roles.Owner);

				if (!createResult.Succeeded || !addToResult.Succeeded)
					throw new ApplicationException("Seeding error: Couldn't create owner!");
			}
		}
	}
}
