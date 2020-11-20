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
		//Statuses only viewable by the owner:
		private readonly IEnumerable<OrderStatus> restrictedStatusValues = new List<OrderStatus>() { OrderStatus.Paid, OrderStatus.Closed, OrderStatus.Cancelled };

		private RestaurantDbContext DbContext { get; }
		public IUserService UserService { get; }

		public OrderService(RestaurantDbContext dbContext, IUserService userService)
		{
			DbContext = dbContext;
			UserService = userService;
		}

		public async Task<IEnumerable<OrderListModel>> GetOrders(List<OrderStatus> statuses)
		{
			await CheckRightsForStatuses(statuses);

			return (await DbContext.Orders.Include(o => o.Customer).Where(o => statuses.Contains(o.Status) || statuses.Count() == 0).ToListAsync())
				.Select(order => new OrderListModel
				{
					Id = order.Id,
					TableId = order.TableId,
					WaiterId = order.WaiterUserId,
					Status = (int)order.Status,
					CustomerId = order.CustomerId,
					CustomerName = order.Customer?.Name,
					CustomerAddress = order.Customer?.Address,
					CustomerPhoneNumber = order.Customer?.PhoneNumber
				});
		}

		private async Task CheckRightsForStatuses(List<OrderStatus> statuses)
		{
			var role = await UserService.GetCurrentUserRole();
			if (role != Roles.Owner && statuses.Intersect(restrictedStatusValues).Count() > 0)
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		}

		private async Task CheckRightsForStatus(OrderStatus status)
		{
			var role = await UserService.GetCurrentUserRole();
			if (role != Roles.Owner && restrictedStatusValues.Contains(status))
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		}

		private OrderStatus StringToStatus(string statusString)
			=> (OrderStatus)Enum.Parse(typeof(OrderStatus), statusString);

		public List<OrderStatus> GetStatusesFromList(List<string> statusesString)
			=> statusesString.Select(statusString => StringToStatus(statusString)).ToList();

		public async Task<OrderDataModel> GetOrderDetails(int id)
		{
			var order = await DbContext.Orders
										.Include(o => o.Waiter)
										.Include(o => o.Customer)
										.Include(o => o.Voucher)
										.Include(o => o.Table)
											.Where(o => o.Id == id).SingleOrDefaultAsync();

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await CheckRightsForStatus(order.Status);

			return new OrderDataModel()
			{
				Id = order.Id,
				TableId = order.TableId,
				TableCode = order.Table.Code,
				WaiterId = order.WaiterUserId,
				WaiterName = order.Waiter.Name,
				CustomerId = order.CustomerId,
				CustomerName = order.Customer.Name,
				CustomerPhoneNumber = order.Customer.PhoneNumber,
				CustomerAddress = order.Customer.Address,
				VoucherId = order.VoucherId,
				VoucherCode = order.Voucher.Code,
				VoucherDiscountPercentage = order.Voucher.DiscountPercentage,
				VoucherDiscountAmount = order.Voucher.DiscountAmount,
				Status = (int)order.Status,
				InvoicePath = order.InvoicePath
			};
		}

		public async Task ModifyOrder(int id, OrderModModel model)
		{
			var order = await DbContext.Orders.FindAsync(id);

			if (order == null)
				throw new RestaurantNotFoundException("Nem létező rendelés!");

			await CheckRightsForStatus(order.Status);

			order.Status = StringToStatus(model.Status);

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrder(int id)
		{
			await ModifyOrder(id, new OrderModModel() { Status = nameof(OrderStatus.Cancelled) });
		}
	}
}
