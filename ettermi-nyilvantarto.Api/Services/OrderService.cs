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
		private RestaurantDbContext DbContext { get; }
		private IStatusService StatusService { get; }
		private IUserService UserService { get; }

		public OrderService(RestaurantDbContext dbContext, IStatusService statusService, IUserService userService)
		{
			this.DbContext = dbContext;
			this.StatusService = statusService;
			this.UserService = userService;
		}

		public async Task<IEnumerable<OrderListModel>> GetOrders(List<string> statusStrings)
		{
			var statuses = StatusService.GetStatusesFromList<OrderStatus>(statusStrings);

			var role = await UserService.GetCurrentUserRole();

			return (await DbContext.Orders
								.Include(o => o.OrderSession)
								.Where(o => StatusService.CanViewStatus(o.OrderSession.Status, role) && (statuses.Contains(o.Status) || statuses.Count() == 0))
								.ToListAsync())
									.Select(order => new OrderListModel
									{
										Id = order.Id,
										WaiterId = order.WaiterUserId,
										Status = (int)order.Status
									});
		}

		public async Task<OrderDataModel> GetOrderDetails(int id)
		{
			var order = await DbContext.Orders
										.Include(o => o.OrderSession)
										.Include(o => o.Waiter)
										.Include(o => o.Items)
											.ThenInclude(oi => oi.MenuItem)
												.ThenInclude(mi => mi.Category)
										.Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status);

			List<OrderItemListModel> items = new List<OrderItemListModel>();
			order.Items.ForEach(oi =>
			{
				items.Add(new OrderItemListModel()
				{
					OrderItemId = oi.Id,
					MenuItemId = oi.MenuItemId,
					Name = oi.MenuItem.Name,
					Quantity = oi.Quantity,
					Price = oi.MenuItem.Price,
					Comment = oi.Comment,
					MenuItemCategoryId = oi.MenuItem.CategoryId,
					MenuItemCategoryName = oi.MenuItem.Category.Name
				});
			});

			return new OrderDataModel()
			{
				Id = order.Id,
				WaiterId = order.WaiterUserId,
				WaiterName = order.Waiter.Name,
				Status = (int)order.Status,
				Items = items
			};
		}

		public async Task<int> AddOrder(OrderAddModel model)
		{
			if (model.TableId == null && model.CustomerId == null)
				throw new RestaurantBadRequestException("Üres rendelés nem vehető fel!");

			OrderSession orderSession = null;
			if(model.TableId != null)
				orderSession = await DbContext.OrderSessions.Where(os => os.TableId == model.TableId && os.Status == OrderSessionStatus.Active).SingleOrDefaultAsync();

			if (orderSession == null)
				orderSession = await CreateNewSession(model);

			var order = DbContext.Orders.Add(new Order()
			{
				WaiterUserId = model.WaiterId,
				Status = OrderStatus.Ordered,
				OrderSessionId = orderSession.Id
			});

			await DbContext.SaveChangesAsync();

			return order.Entity.Id;
		}

		private async Task<OrderSession> CreateNewSession(OrderAddModel model)
		{
			var orderSession = DbContext.OrderSessions.Add(new OrderSession()
			{
				TableId = model.TableId,
				CustomerId = model.CustomerId,
				Status = OrderSessionStatus.Active
			});

			await DbContext.SaveChangesAsync();

			return orderSession.Entity;
		}

		public async Task ModifyOrder(int id, StatusModModel model)
		{
			var order = await DbContext.Orders.Include(o => o.OrderSession).Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status);

			order.Status = StatusService.StringToStatus<OrderStatus>(model.Status);

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrder(int id)
		{
			await ModifyOrder(id, new StatusModModel() { Status = nameof(OrderStatus.Cancelled) });
		}
	}
}
