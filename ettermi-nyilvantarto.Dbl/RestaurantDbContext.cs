using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ettermi_nyilvantarto.Dbl
{
	public class RestaurantDbContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
		public RestaurantDbContext(DbContextOptions options) : base(options)
		{

		}
	}
}
