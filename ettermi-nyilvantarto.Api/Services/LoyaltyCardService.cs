using ettermi_nyilvantarto.Dbl;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class LoyaltyCardService : ILoyaltyCardService
	{
		private RestaurantDbContext DbContext { get; }
		public LoyaltyCardService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<int> GetLoyaltyCardBalance(int cardNumber)
		{
			var card = await DbContext.LoyaltyCards.Where(lc => lc.CardNumber == cardNumber).SingleOrDefaultAsync();

			if (card == null)
				throw new RestaurantNotFoundException("Nem létező kártya");

			return card.Points;
		} 
	}
}
