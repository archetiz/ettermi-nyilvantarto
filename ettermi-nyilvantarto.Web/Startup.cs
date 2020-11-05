using ettermi_nyilvantarto.Api;
using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using ettermi_nyilvantarto.Dbl.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueCliMiddleware;

namespace ettermi_nyilvantarto
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<RestaurantDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("RestaurantConnectionString")));

			services.AddIdentity<User, IdentityRole<int>>().AddEntityFrameworkStores<RestaurantDbContext>().AddDefaultTokenProviders();

			services.AddScoped<IRoleSeedService, RoleSeedService>();
			services.AddScoped<IUserSeedService, UserSeedService>();

			services.AddScoped<IUserService, UserService>();

			services.AddControllers();
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp";
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseSpaStaticFiles();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseSpa(spa =>
			{
				if (env.IsDevelopment())
					spa.Options.SourcePath = "ClientApp";
				else
					spa.Options.SourcePath = "dist";

				if (env.IsDevelopment())
				{
					spa.UseVueCli(npmScript: "serve", port: 8089);
				}

			});
		}
	}
}
