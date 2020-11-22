using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IOrderSessionService
	{
		Task<IEnumerable<OrderSessionListModel>> GetOrderSessions(List<string> statusStrings);
		Task<OrderSessionDataModel> GetOrderSessionDetails(int id);
		Task ModifyOrderSessionStatus(int id, StatusModModel model);
		Task CancelOrderSession(int id);
		Task<int> PayOrders(int id, OrderSessionPayModel model);
	}
}
