using Microsoft.AspNetCore.Identity;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class User : IdentityUser<int>
	{
		public string Name { get; set; }
	}
}
