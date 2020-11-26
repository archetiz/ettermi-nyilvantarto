using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IOrderSessionService
	{
		Task<PagedResult<OrderSessionListModel>> GetOrderSessions(List<string> statusStrings, int page);
		Task<OrderSessionDataModel> GetOrderSessionDetails(int id);
		Task<OrderSession> CreateNewSession(OrderAddModel model);
		Task ModifyOrderSessionStatus(int id, StatusModModel model);
		Task CancelOrderSession(int id);
		Task<int> PayOrders(int id, OrderSessionPayModel model);
	}
}
