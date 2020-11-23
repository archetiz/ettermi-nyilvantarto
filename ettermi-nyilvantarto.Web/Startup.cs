using ettermi_nyilvantarto.Api;
using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using ettermi_nyilvantarto.Dbl.Seed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using VueCliMiddleware;

namespace ettermi_nyilvantarto
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		public IConfiguration Configuration { get; }
		public IWebHostEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<RestaurantDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("RestaurantConnectionString")));

			services.AddIdentity<User, IdentityRole<int>>().AddEntityFrameworkStores<RestaurantDbContext>().AddDefaultTokenProviders();

			static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
				context => {
					if (context.Request.Path.StartsWithSegments("/api"))
					{
						context.Response.StatusCode = (int)statusCode;
						return Task.CompletedTask;
					}
					return existingRedirector(context);
				};

			services.ConfigureApplicationCookie(o =>
			{
				o.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, o.Events.OnRedirectToAccessDenied);
				o.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, o.Events.OnRedirectToLogin);
			});

			services.AddScoped<IRoleSeedService, RoleSeedService>();
			services.AddScoped<IUserSeedService, UserSeedService>();

			services.AddScoped<ICustomerService, CustomerService>();
			services.AddScoped<IFeedbackService, FeedbackService>();
			services.AddScoped<ILoyaltyCardService, LoyaltyCardService>();
			services.AddScoped<IMenuService, MenuService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<IOrderSessionService, OrderSessionService>();
			services.AddScoped<IReservationService, ReservationService>();
			services.AddScoped<IStatusService, StatusService>();
			services.AddScoped<ITableService, TableService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IVoucherService, VoucherService>();

			services.Configure<OwnerConfiguration>(Configuration.GetSection("Owner"));
			services.Configure<OrderConfiguration>(Configuration.GetSection("Order"));

			services.AddControllers();
			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp";
			});

			if (Environment.IsDevelopment())
			{
				services.AddSwaggerGen();
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Doc");
				});
			}
			app.UseExceptionHandler("/error");

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
