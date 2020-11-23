using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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
		private IOrderSessionService OrderSessionService { get; }

		public OrderService(RestaurantDbContext dbContext, IStatusService statusService, IUserService userService, IOrderSessionService orderSessionService)
		{
			this.DbContext = dbContext;
			this.StatusService = statusService;
			this.UserService = userService;
			this.OrderSessionService = orderSessionService;
		}

		public async Task<IEnumerable<OrderListModel>> GetOrders(List<string> statusStrings)
		{
			var statuses = StatusService.GetStatusesFromList<OrderStatus>(statusStrings);

			var role = await UserService.GetCurrentUserRole();

			return (await DbContext.Orders
								.Include(o => o.OrderSession)
								.Where(o => StatusService.CanViewStatus(o.OrderSession.Status, role) && (statuses.Contains(o.Status) || statuses.Count() == 0))
								.OrderBy(o => o.ClosedAt ?? DateTime.MinValue).ThenBy(o => o.OpenedAt)
								.ToListAsync())
									.Select(order => new OrderListModel
									{
										Id = order.Id,
										WaiterId = order.WaiterUserId,
										Status = (int)order.Status,
										OpenedAt = order.OpenedAt,
										ClosedAt = order.ClosedAt
									});
		}

		public async Task<OrderDataModel> GetOrderDetails(int id)
		{
			var order = await DbContext.Orders
										.Include(o => o.OrderSession)
											.ThenInclude(os => os.Table)
										.Include(o => o.OrderSession.Customer)
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
				OrderSessionId = order.OrderSessionId,
				WaiterId = order.WaiterUserId,
				WaiterName = order.Waiter.Name,
				TableId = order.OrderSession.TableId,
				TableCode = order.OrderSession.Table.Code,
				CustomerId = order.OrderSession.CustomerId,
				CustomerName = order.OrderSession.Customer.Name,
				CustomerPhoneNumber = order.OrderSession.Customer.PhoneNumber,
				CustomerAddress = order.OrderSession.Customer.Address,
				Status = (int)order.Status,
				OpenedAt = order.OpenedAt,
				ClosedAt = order.ClosedAt,
				Items = items
			};
		}

		public async Task<int> AddOrder(OrderAddModel model)
		{
			if (model.TableId == null && model.CustomerId == null)
				throw new RestaurantBadRequestException("Asztal/kiszállítási adatok nélküli rendelés nem vehető fel!");

			OrderSession orderSession = null;
			if(model.TableId != null)
				orderSession = await DbContext.OrderSessions.Where(os => os.TableId == model.TableId && os.Status == OrderSessionStatus.Active).SingleOrDefaultAsync();

			if (orderSession == null)
				orderSession = await OrderSessionService.CreateNewSession(model);

			var order = DbContext.Orders.Add(new Order()
			{
				WaiterUserId = model.WaiterId,
				Status = OrderStatus.Ordered,
				OrderSessionId = orderSession.Id,
				OpenedAt = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return order.Entity.Id;
		}

		public async Task ModifyOrder(int id, StatusModModel model)
		{
			var order = await DbContext.Orders.Include(o => o.OrderSession).Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status);

			order.Status = StatusService.StringToStatus<OrderStatus>(model.Status);

			if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Served)
				order.ClosedAt = DateTime.Now;

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrder(int id)
		{
			await ModifyOrder(id, new StatusModModel() { Status = nameof(OrderStatus.Cancelled) });
		}

		public async Task<int> AddOrderItem(int orderId, OrderItemAddModel model)
		{
			var order = await DbContext.Orders.Include(o => o.OrderSession)
												.Where(o => o.Id == orderId && o.Status == OrderStatus.Ordered && o.OrderSession.Status == OrderSessionStatus.Active)
												.SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("A rendelés nem létezik vagy nem lehetséges új tételek hozzáadása!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status);

			var orderItem = DbContext.OrderItems.Add(new OrderItem()
			{
				OrderId = orderId,
				MenuItemId = model.MenuItemId,
				Quantity = model.Quantity,
				Comment = model.Comment
			});

			await DbContext.SaveChangesAsync();

			return orderItem.Entity.Id;
		}

		public async Task ModifyOrderItem(int orderId, int itemId, OrderItemModModel model)
		{
			var orderItem = await DbContext.OrderItems.Where(oi => oi.Id == itemId && oi.OrderId == orderId).SingleOrDefaultAsync();

			if (orderItem == null)
				throw new RestaurantNotFoundException("Nem létező rendelési tétel!");

			orderItem.Quantity = model.Quantity;
			orderItem.Comment = model.Comment;

			await DbContext.SaveChangesAsync();
		}

		public async Task RemoveOrderItem(int orderId, int itemId)
		{
			var order = await DbContext.Orders
									.Include(o => o.OrderSession)
									.Include(o => o.Items)
									.Where(o => o.Id == orderId && o.Status == OrderStatus.Ordered && o.OrderSession.Status == OrderSessionStatus.Active)
									.SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("A rendelés nem létezik vagy nem lehetséges a módosítása!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status);

			DbContext.OrderItems.Remove(order.Items.Find(oi => oi.Id == itemId));

			await DbContext.SaveChangesAsync();
		}
	}
}
