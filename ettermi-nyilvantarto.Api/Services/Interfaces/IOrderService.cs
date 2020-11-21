using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IOrderService
	{
		Task<IEnumerable<OrderListModel>> GetOrders(List<OrderStatus> statuses);
		List<OrderStatus> GetStatusesFromList(List<string> statusesString);
		Task<OrderDataModel> GetOrderDetails(int id);
		Task<int> AddOrder(OrderAddModel model);
		Task ModifyOrder(int id, OrderModModel model);
		Task CancelOrder(int id);
		Task<int> PayOrder(int id, OrderPayModel model);
	}
}
