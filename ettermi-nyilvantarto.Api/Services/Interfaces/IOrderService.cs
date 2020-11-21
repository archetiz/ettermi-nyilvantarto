using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IOrderService
	{
		Task<IEnumerable<OrderListModel>> GetOrders(List<string> statusStrings);
		Task<OrderDataModel> GetOrderDetails(int id);
		Task<int> AddOrder(OrderAddModel model);
		Task ModifyOrder(int id, StatusModModel model);
		Task CancelOrder(int id);
	}
}
