using ettermi_nyilvantarto.Dbl.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IOrderService
	{
		Task<PagedResult<OrderListModel>> GetOrders(List<string> statusStrings, int page);
		Task<OrderDataModel> GetOrderDetails(int id);
		Task<int> AddOrder(OrderAddModel model);
		Task ModifyOrder(int id, StatusModModel model);
		Task CancelOrder(int id);
		Task<int> AddOrderItem(int orderId, OrderItemAddModel model);
		Task ModifyOrderItem(int orderId, int itemId, OrderItemModModel model);
		Task RemoveOrderItem(int orderId, int itemId);
	}
}
