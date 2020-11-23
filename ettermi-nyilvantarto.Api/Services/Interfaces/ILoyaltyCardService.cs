using ettermi_nyilvantarto.Dbl.Entities;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ILoyaltyCardService
	{
		Task<int> GetLoyaltyCardBalance(int cardNumber);
		Task<LoyaltyCard> AddLoyaltyCard(int cardNumber);
	}
}
