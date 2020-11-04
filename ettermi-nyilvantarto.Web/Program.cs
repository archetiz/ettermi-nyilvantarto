using System.Threading.Tasks;
using ettermi_nyilvantarto.Dbl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ettermi_nyilvantarto
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			(await CreateHostBuilder(args)
				.Build()
				.MigrateDatabse<RestaurantDbContext>())
				.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
