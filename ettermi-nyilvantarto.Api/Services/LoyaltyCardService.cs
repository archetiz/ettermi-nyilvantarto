using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
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

		public async Task<LoyaltyCardBalanceModel> GetLoyaltyCardBalance(int cardNumber)
		{
			var card = await DbContext.LoyaltyCards.Where(lc => lc.CardNumber == cardNumber).SingleOrDefaultAsync();

			if (card == null)
				throw new RestaurantNotFoundException("Nem létező kártya");

			return new LoyaltyCardBalanceModel() { Balance = card.Points };
		}

		public async Task<LoyaltyCard> AddLoyaltyCard(int cardNumber)
		{
			if (cardNumber <= 0)
				throw new RestaurantBadRequestException("A kártya kódjának pozitív számnak kell lennie!");

			var existingCard = await DbContext.LoyaltyCards.Where(lc => lc.CardNumber == cardNumber).SingleOrDefaultAsync();

			if (existingCard != null)
				throw new RestaurantBadRequestException("Sikertelen hűségkártya felvétel: már létezik kártya ezzel az azonosítóval!");

			var loyaltyCard = DbContext.LoyaltyCards.Add(new LoyaltyCard()
			{
				CardNumber = cardNumber,
				Points = 0
			});

			await DbContext.SaveChangesAsync();

			return loyaltyCard.Entity;
		}
	}
}
