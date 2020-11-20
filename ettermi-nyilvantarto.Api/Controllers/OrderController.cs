using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/order")]
	[Produces("application/json")]
	public class OrderController : ControllerBase
	{
		private IOrderService OrderService { get; set; }
		public OrderController(IOrderService orderService)
		{
			this.OrderService = orderService;
		}

		[HttpGet]
		[Authorize]
		public async Task<IEnumerable<OrderListModel>> ListOrders([FromBody] List<string> statuses)
			=> await OrderService.GetOrders(OrderService.GetStatusesFromList(statuses));

		[HttpGet("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<OrderDataModel> GetOrderData(int id)
			=> await OrderService.GetOrderDetails(id);

		[HttpPut("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task ModifyOrder(int id, OrderModModel order)
			=> await OrderService.ModifyOrder(id, order);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task CancelOrder(int id)
			=> await OrderService.CancelOrder(id);
	}
}
