using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Dbl.Seed
{
	public class RoleSeedService : IRoleSeedService
	{
		private readonly RoleManager<IdentityRole<int>> roleManager;

		public RoleSeedService(RoleManager<IdentityRole<int>> roleManager)
		{
			this.roleManager = roleManager;
		}

		public async Task SeedRoleAsync()
		{
			if (!await roleManager.RoleExistsAsync(Roles.Owner))
				await roleManager.CreateAsync(new IdentityRole<int> { Name = Roles.Owner });

			if (!await roleManager.RoleExistsAsync(Roles.Chef))
				await roleManager.CreateAsync(new IdentityRole<int> { Name = Roles.Chef });

			if (!await roleManager.RoleExistsAsync(Roles.Waiter))
				await roleManager.CreateAsync(new IdentityRole<int> { Name = Roles.Waiter });
		}
	}
}
