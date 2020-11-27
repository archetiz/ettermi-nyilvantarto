using ettermi_nyilvantarto.Dbl.Entities;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ILoyaltyCardService
	{
		Task<LoyaltyCardBalanceModel> GetLoyaltyCardBalance(int cardNumber);
		Task<LoyaltyCard> AddLoyaltyCard(int cardNumber);
	}
}
