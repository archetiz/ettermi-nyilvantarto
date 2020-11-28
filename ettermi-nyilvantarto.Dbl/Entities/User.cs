using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class User : IdentityUser<int>
	{
		public string Name { get; set; }
		public bool IsActive { get; set; } = true;
		public List<Order> Orders { get; set; }
	}
}
