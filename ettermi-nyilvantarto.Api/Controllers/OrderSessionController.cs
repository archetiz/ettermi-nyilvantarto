using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/orders")]
	[Produces("application/json")]
	public class OrderSessionController : ControllerBase
	{
		private IOrderSessionService OrderSessionService { get; }
		public OrderSessionController(IOrderSessionService orderSessionService)
		{
			this.OrderSessionService = orderSessionService;
		}

		[HttpGet]
		[HttpGet("page/{page}")]
		[Authorize]
		public async Task<PagedResult<OrderSessionListModel>> ListOrderSession([FromQuery] List<string> statuses, int page = 1)
			=> await OrderSessionService.GetOrderSessions(statuses, page);

		[HttpGet("{id}")]
		[Authorize]
		public async Task<OrderSessionDataModel> GetOrderSessionData(int id)
			=> await OrderSessionService.GetOrderSessionDetails(id);

		[HttpPut("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task ModifyOrderSessionStatus(int id, StatusModModel status)
			=> await OrderSessionService.ModifyOrderSessionStatus(id, status);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task CancelOrderSession(int id)
			=> await OrderSessionService.CancelOrderSession(id);

		[HttpPost("{id}/pay")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<OrderSessionPayResultModel> PayOrderSession(int id, OrderSessionPayModel order)
			=> await OrderSessionService.PayOrders(id, order);
	}
}
