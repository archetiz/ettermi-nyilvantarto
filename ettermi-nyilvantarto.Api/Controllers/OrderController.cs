﻿using Microsoft.AspNetCore.Authorization;
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
		private IOrderService OrderService { get; }
		public OrderController(IOrderService orderService)
		{
			this.OrderService = orderService;
		}

		[HttpGet]
		[Authorize]
		public async Task<IEnumerable<OrderListModel>> ListOrders([FromBody] List<string> statuses)
			=> await OrderService.GetOrders(statuses);

		[HttpGet("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<OrderDataModel> GetOrderData(int id)
			=> await OrderService.GetOrderDetails(id);

		[HttpPost]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<int> AddOrder(OrderAddModel order)
			=> await OrderService.AddOrder(order);

		[HttpPut("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task ModifyOrder(int id, StatusModModel status)
			=> await OrderService.ModifyOrder(id, status);

		[HttpDelete("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task CancelOrder(int id)
			=> await OrderService.CancelOrder(id);
	}
}
