using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/loyalty/card")]
	[Produces("application/json")]
	[Authorize(Roles = "Owner,Waiter")]
	public class LoyaltyCardController
	{
		private ILoyaltyCardService LoyaltyCardService { get; }
		public LoyaltyCardController(ILoyaltyCardService loyaltyCardService)
		{
			this.LoyaltyCardService = loyaltyCardService;
		}

		[HttpGet("{cardNumber}")]
		public async Task<int> GetCardBalance(int cardNumber)
			=> await LoyaltyCardService.GetLoyaltyCardBalance(cardNumber);
	}
}
