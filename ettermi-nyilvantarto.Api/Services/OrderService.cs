using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class OrderService : IOrderService
	{
		//Statuses only viewable by the owner:
		//private readonly IEnumerable<OrderStatus> restrictedStatusValues = new List<OrderStatus>() { OrderStatus.Paid, OrderStatus.Closed, OrderStatus.Cancelled };

		private RestaurantDbContext DbContext { get; }
		public IUserService UserService { get; }
		public IStatusService StatusService { get; set; }

		public OrderService(RestaurantDbContext dbContext, IUserService userService, IStatusService statusService)
		{
			DbContext = dbContext;
			UserService = userService;
			StatusService = statusService;
		}

		public async Task<IEnumerable<OrderListModel>> GetOrders(List<string> statusStrings)
		{
			var statuses = StatusService.GetStatusesFromList<OrderStatus>(statusStrings);

			await CheckRightsForStatuses(statuses);

			return (await DbContext.Orders.Where(o => statuses.Contains(o.Status) || statuses.Count() == 0).ToListAsync())
				.Select(order => new OrderListModel
				{
					Id = order.Id,
					WaiterId = order.WaiterUserId,
					Status = (int)order.Status
				});
		}

		//private async Task CheckRightsForStatuses(List<OrderStatus> statuses)
		//{
		//	var role = await UserService.GetCurrentUserRole();
		//	if (role != Roles.Owner && statuses.Intersect(restrictedStatusValues).Count() > 0)
		//		throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		//}

		//private async Task CheckRightsForStatus(OrderStatus status)
		//{
		//	var role = await UserService.GetCurrentUserRole();
		//	if (role != Roles.Owner && restrictedStatusValues.Contains(status))
		//		throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		//}

		public async Task<OrderDataModel> GetOrderDetails(int id)
		{
			var order = await DbContext.Orders.Include(o => o.Waiter).Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await CheckRightsForStatus(order.Status);

			return new OrderDataModel()
			{
				Id = order.Id,
				WaiterId = order.WaiterUserId,
				WaiterName = order.Waiter.Name,
				Status = (int)order.Status
				//TODO: Items
			};
		}

		public async Task<int> AddOrder(OrderAddModel model)		//TODO
		{
			var order = DbContext.Orders.Add(new Order()
			{
				WaiterUserId = model.WaiterId,
				TableId = model.TableId,
				CustomerId = model.CustomerId,
				Status = OrderStatus.Ordered
			});

			await DbContext.SaveChangesAsync();

			return order.Entity.Id;
		}

		public async Task ModifyOrder(int id, StatusModModel model)
		{
			var order = await DbContext.Orders.FindAsync(id);

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await CheckRightsForStatus(order.Status);

			order.Status = StatusService.StringToStatus<OrderStatus>(model.Status);

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrder(int id)
		{
			await ModifyOrder(id, new StatusModModel() { Status = nameof(OrderStatus.Cancelled) });
		}
	}
}
