using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/orders")]
	[Produces("application/json")]
	[Authorize(Roles = "Owner,Waiter")]
	public class OrderSessionController : ControllerBase
	{
		private IOrderSessionService OrderSessionService { get; }
		public OrderSessionController(IOrderSessionService orderSessionService)
		{
			this.OrderSessionService = orderSessionService;
		}

		[HttpGet]
		public async Task<IEnumerable<OrderSessionListModel>> ListOrderSession([FromBody] List<string> statuses)
			=> await OrderSessionService.GetOrderSessions(statuses);

		[HttpGet("{id}")]
		public async Task<OrderSessionDataModel> GetOrderSessionData(int id)
			=> await OrderSessionService.GetOrderSessionDetails(id);

		[HttpPut("{id}")]
		public async Task ModifyOrderSessionStatus(int id, StatusModModel status)
			=> await OrderSessionService.ModifyOrderSessionStatus(id, status);

		[HttpDelete("{id}")]
		public async Task CancelOrderSession(int id)
			=> await OrderSessionService.CancelOrderSession(id);

		[HttpPut("{id}/pay")]
		public async Task PayOrderSession(int id, OrderSessionPayModel order)
			=> await OrderSessionService.PayOrders(id, order);
	}
}
