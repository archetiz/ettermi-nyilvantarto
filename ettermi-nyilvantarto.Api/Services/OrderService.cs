using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
		private PagingConfiguration PagingConfig { get; }

		public OrderService(RestaurantDbContext dbContext, IStatusService statusService, IUserService userService, IOrderSessionService orderSessionService, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.StatusService = statusService;
			this.UserService = userService;
			this.OrderSessionService = orderSessionService;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<PagedResult<OrderListModel>> GetOrders(List<string> statusStrings, int page)
		{
			var statuses = StatusService.GetStatusesFromList<OrderStatus>(statusStrings);

			var role = await UserService.GetCurrentUserRole();

			return DbContext.Orders
								.Include(o => o.OrderSession)
									.ThenInclude(os => os.Table)
								.Include(o => o.Waiter)
								.Include(o => o.Items)
									.ThenInclude(oi => oi.MenuItem)
								.AsEnumerable()
								.Where(o => StatusService.CanViewStatus(role, o.OrderSession.Status, o.Status) && (statuses.Contains(o.Status) || statuses.Count() == 0))
								.OrderBy(o => o.ClosedAt ?? DateTime.MinValue).ThenBy(o => o.OpenedAt)
								.GetPaged(page, PagingConfig.PageSize, out int totalPages)
								.Select(order => new OrderListModel
								{
									Id = order.Id,
									TableId = order.OrderSession.TableId,
									TableCode = order.OrderSession.Table?.Code,
									WaiterId = order.WaiterUserId,
									WaiterName = order.Waiter.Name,
									Status = Enum.GetName(typeof(OrderStatus), order.Status),
									OpenedAt = order.OpenedAt,
									ClosedAt = order.ClosedAt,
									Price = order.CalculatePrice()
								}).ToList().GetPagedResult(page, PagingConfig.PageSize, totalPages);
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

			await StatusService.CheckRightsForStatus(order.OrderSession.Status, order.Status);

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
				TableCode = order.OrderSession.Table?.Code,
				CustomerId = order.OrderSession.CustomerId,
				CustomerName = order.OrderSession.Customer?.Name,
				CustomerPhoneNumber = order.OrderSession.Customer?.PhoneNumber,
				CustomerAddress = order.OrderSession.Customer?.Address,
				Status = Enum.GetName(typeof(OrderStatus), order.Status),
				OpenedAt = order.OpenedAt,
				ClosedAt = order.ClosedAt,
				Price = order.CalculatePrice(),
				Items = items
			};
		}

		public async Task<AddResult> AddOrder(OrderAddModel model)
		{
			if (model.TableId == null && model.CustomerId == null)
				throw new RestaurantBadRequestException("Asztal/kiszállítási adatok nélküli rendelés nem vehető fel!");

			var waiter = await DbContext.Users.FindAsync(model.WaiterId);
			if (waiter == null)
				throw new RestaurantNotFoundException("A megadott pincér nem létezik!");

			await StatusService.CheckRightsForOrderAddDelete();

			OrderSession orderSession = null;
			if(model.TableId != null)
				orderSession = await DbContext.OrderSessions.Where(os => os.TableId == model.TableId && os.Status == OrderSessionStatus.Active).SingleOrDefaultAsync();

			if (orderSession == null)
				orderSession = await OrderSessionService.CreateNewSession(model);

			var order = DbContext.Orders.Add(new Order()
			{
				WaiterUserId = model.WaiterId,
				Status = OrderStatus.Ordering,
				OrderSessionId = orderSession.Id,
				OpenedAt = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(order.Entity.Id);
		}

		public async Task ModifyOrder(int id, StatusModModel model)
		{
			if (!Enum.IsDefined(typeof(OrderStatus), model.Status))
				throw new RestaurantNotFoundException("Nem létező státusz!");

			var order = await DbContext.Orders.Include(o => o.OrderSession).Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			var newStatus = StatusService.StringToStatus<OrderStatus>(model.Status);

			await StatusService.CheckRightsForStatusModification(order.OrderSession.Status, order.Status, newStatus);

			order.Status = newStatus;

			if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Served)
				order.ClosedAt = DateTime.Now;

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrder(int id)
		{
			await StatusService.CheckRightsForOrderAddDelete();
			await ModifyOrder(id, new StatusModModel() { Status = nameof(OrderStatus.Cancelled) });
		}

		public async Task<AddResult> AddOrderItem(int orderId, OrderItemAddModel model)
		{
			if (model.Quantity < 1)
				throw new RestaurantBadRequestException("A mennyiségnek pozitív számnak kell lennie!");

			var menuItem = await DbContext.MenuItems.FindAsync(model.MenuItemId);
			if (menuItem == null)
				throw new RestaurantNotFoundException("A megadott étel/ital nem létezik!");

			var order = await DbContext.Orders.Include(o => o.OrderSession)
												.Where(o => o.Id == orderId && o.Status == OrderStatus.Ordering && o.OrderSession.Status == OrderSessionStatus.Active)
												.SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("A rendelés nem létezik vagy nem lehetséges új tételek hozzáadása!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status, order.Status);

			var orderItem = DbContext.OrderItems.Add(new OrderItem()
			{
				OrderId = orderId,
				MenuItemId = model.MenuItemId,
				Quantity = model.Quantity,
				Comment = model.Comment
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(orderItem.Entity.Id);
		}

		public async Task ModifyOrderItem(int orderId, int itemId, OrderItemModModel model)
		{
			if (model.Quantity < 1)
				throw new RestaurantBadRequestException("A mennyiségnek pozitív számnak kell lennie!");

			var orderItem = await DbContext.OrderItems
													.Include(oi => oi.Order)
														.ThenInclude(o => o.OrderSession)
													.Where(oi => oi.Id == itemId && oi.OrderId == orderId
															&& oi.Order.Status == OrderStatus.Ordering && oi.Order.OrderSession.Status == OrderSessionStatus.Active)
													.SingleOrDefaultAsync();

			if (orderItem == null)
				throw new RestaurantNotFoundException("Nem létező rendelési tétel!");

			await StatusService.CheckRightsForStatus(orderItem.Order.OrderSession.Status, orderItem.Order.Status);

			orderItem.Quantity = model.Quantity;
			orderItem.Comment = model.Comment;

			await DbContext.SaveChangesAsync();
		}

		public async Task RemoveOrderItem(int orderId, int itemId)
		{
			var order = await DbContext.Orders
									.Include(o => o.OrderSession)
									.Include(o => o.Items)
									.Where(o => o.Id == orderId && o.Status == OrderStatus.Ordering && o.OrderSession.Status == OrderSessionStatus.Active)
									.SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("A rendelés nem létezik vagy nem lehetséges a módosítása!");

			await StatusService.CheckRightsForStatus(order.OrderSession.Status, order.Status);

			DbContext.OrderItems.Remove(order.Items.Find(oi => oi.Id == itemId));

			await DbContext.SaveChangesAsync();
		}
	}
}
