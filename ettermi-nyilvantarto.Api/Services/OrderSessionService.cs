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
	public class OrderSessionService : IOrderSessionService
	{
		private RestaurantDbContext DbContext { get; }
		private IStatusService StatusService { get; }
		private OrderConfiguration OrderConfig { get; }
		private ILoyaltyCardService LoyaltyCardService { get; }
		private PagingConfiguration PagingConfig { get; }

		public OrderSessionService(RestaurantDbContext dbContext, IStatusService statusService, IOptions<OrderConfiguration> config, ILoyaltyCardService loyaltyCardService, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.StatusService = statusService;
			this.OrderConfig = config.Value;
			this.LoyaltyCardService = loyaltyCardService;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<PagedResult<OrderSessionListModel>> GetOrderSessions(List<string> statusStrings, int page)
		{
			var statuses = StatusService.GetStatusesFromList<OrderSessionStatus>(statusStrings);

			await StatusService.CheckRightsForStatuses(statuses);

			return DbContext.OrderSessions
						.Include(os => os.Table)
						.Include(os => os.Customer)
						.Include(os => os.Orders)
							.ThenInclude(o => o.Items)
								.ThenInclude(oi => oi.MenuItem)
						.Where(os => statuses.Contains(os.Status) || statuses.Count() == 0)
						.OrderBy(os => os.ClosedAt ?? DateTime.MinValue).ThenBy(os => os.OpenedAt)
						.GetPaged(page, PagingConfig.PageSize, out int totalPages)
						.AsEnumerable()
						.Select(os => new OrderSessionListModel()
						{
							Id = os.Id,
							TableId = os.TableId,
							TableCode = os.Table.Code,
							CustomerId = os.CustomerId,
							CustomerName = os.Customer.Name,
							VoucherId = os.VoucherId,
							InvoiceId = os.InvoiceId,
							Status = Enum.GetName(typeof(OrderSessionStatus), os.Status),
							OpenedAt = os.OpenedAt,
							ClosedAt = os.ClosedAt,
							FullPrice = CalculatePrice(os)
						}).ToList().GetPagedResult(page, PagingConfig.PageSize, totalPages);
		}

		public async Task<OrderSessionDataModel> GetOrderSessionDetails(int id)
		{
			var orderSession = await DbContext.OrderSessions
										.Include(os => os.Customer)
										.Include(os => os.Voucher)
										.Include(os => os.Table)
										.Include(os => os.Orders)
											.ThenInclude(o => o.Items)
												.ThenInclude(oi => oi.MenuItem)
										.Where(os => os.Id == id).SingleOrDefaultAsync();

			if (orderSession == null)
				throw new RestaurantNotFoundException("Nem létező rendelési folyamat!");

			await StatusService.CheckRightsForStatus(orderSession.Status);

			var orders = new List<OrderListModel>();
			orderSession.Orders.ForEach(order =>
			{
				orders.Add(new OrderListModel()
				{
					Id = order.Id,
					WaiterId = order.WaiterUserId,
					Status = Enum.GetName(typeof(OrderStatus), order.Status),
					OpenedAt = order.OpenedAt,
					ClosedAt = order.ClosedAt,
					Price = order.CalculatePrice()
				});
			});

			return new OrderSessionDataModel()
			{
				Id = orderSession.Id,
				TableId = orderSession.TableId,
				TableCode = orderSession.Table.Code,
				CustomerId = orderSession.CustomerId,
				CustomerName = orderSession.Customer.Name,
				CustomerPhoneNumber = orderSession.Customer.PhoneNumber,
				CustomerAddress = orderSession.Customer.Address,
				VoucherId = orderSession.VoucherId,
				VoucherCode = orderSession.Voucher.Code,
				VoucherDiscountPercentage = orderSession.Voucher.DiscountPercentage,
				VoucherDiscountAmount = orderSession.Voucher.DiscountAmount,
				InvoiceId = orderSession.InvoiceId,
				Status = Enum.GetName(typeof(OrderSessionStatus), orderSession.Status),
				OpenedAt = orderSession.OpenedAt,
				ClosedAt = orderSession.ClosedAt,
				FullPrice = CalculatePrice(orderSession),
				Orders = orders
			};
		}

		public async Task<OrderSession> CreateNewSession(OrderAddModel model)
		{
			var orderSession = DbContext.OrderSessions.Add(new OrderSession()
			{
				TableId = model.TableId,
				CustomerId = model.CustomerId,
				Status = OrderSessionStatus.Active,
				OpenedAt = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return orderSession.Entity;
		}

		public async Task ModifyOrderSessionStatus(int id, StatusModModel model)
		{
			var orderSession = await DbContext.OrderSessions.Include(os => os.Orders).Where(os => os.Id == id).SingleOrDefaultAsync();

			if (orderSession == null)
				throw new RestaurantNotFoundException("Nem létező rendelési folyamat!");

			await StatusService.CheckRightsForStatus(orderSession.Status);

			orderSession.Status = StatusService.StringToStatus<OrderSessionStatus>(model.Status);

			if (orderSession.Status == OrderSessionStatus.Cancelled || orderSession.Status == OrderSessionStatus.Paid)
			{
				orderSession.ClosedAt = DateTime.Now;
				if (orderSession.Status == OrderSessionStatus.Cancelled)
				{
					orderSession.Orders.ForEach(order =>
					{
						order.Status = OrderStatus.Cancelled;
						order.ClosedAt = DateTime.Now;
					});
				}
			}

			await DbContext.SaveChangesAsync();
		}

		public async Task CancelOrderSession(int id)
		{
			await ModifyOrderSessionStatus(id, new StatusModModel() { Status = nameof(OrderSessionStatus.Cancelled) });
		}

		public async Task<int> PayOrders(int id, OrderSessionPayModel model)
		{
			var orderSession = await DbContext.OrderSessions.Include(os => os.Orders)
																.ThenInclude(o => o.Items)
																	.ThenInclude(oi => oi.MenuItem)
															.Where(os => os.Id == id && (os.Status == OrderSessionStatus.Active || os.Status == OrderSessionStatus.Delivering))
															.SingleOrDefaultAsync();

			if (orderSession == null)
				throw new RestaurantNotFoundException("A rendelési folyamat nem létezik vagy a fizetése nem lehetséges!");

			await StatusService.CheckRightsForStatus(orderSession.Status);

			//Calculate base price
			var price = CalculatePrice(orderSession);
			var fullPrice = price;

			//Note: calculation order matters here, we redeem loyalty points 1st, because if the vouchers are percentage based, overall this results in potentially higher net gain for the restaurant

			//Loyalty points
			if (model.LoyaltyCardNumber != null)
			{
				var loyaltyCard = await DbContext.LoyaltyCards.Where(lc => lc.CardNumber == model.LoyaltyCardNumber).SingleOrDefaultAsync();

				if (loyaltyCard == null)
					loyaltyCard = await LoyaltyCardService.AddLoyaltyCard(model.LoyaltyCardNumber ?? 1);

				//Redeem points
				var redeemedPoints = model.RedeemedPoints ?? 0;
				if (redeemedPoints > 0)
				{
					if (loyaltyCard.Points < redeemedPoints)
						throw new RestaurantBadRequestException("Nem áll rendelkezésre elegendő hűségpont a beváltáshoz!");

					loyaltyCard.Points -= redeemedPoints;
					price -= redeemedPoints;
				}

				//Add new points for the purchase
				loyaltyCard.Points += (int)Math.Round(fullPrice * OrderConfig.LoyaltyPointsMultiplier);
			}

			//Vouchers
			if (!string.IsNullOrEmpty(model.VoucherCode))
				price = await CalculateVoucherDiscountedPrice(model.VoucherCode, price);

			//Close order
			orderSession.Status = OrderSessionStatus.Paid;
			orderSession.ClosedAt = DateTime.Now;

			//Generate invoice
			//...

			await DbContext.SaveChangesAsync();

			return 0;   //Invoice id
		}

		private int CalculatePrice(OrderSession orderSession)
		{
			int sum = 0;
			orderSession.Orders.ForEach(order =>
			{
				sum += order.CalculatePrice();
			});
			return sum;
		}

		private async Task<int> CalculateVoucherDiscountedPrice(string voucherCode, int price)
		{
			var voucher = await DbContext.Vouchers
											.Where(v => v.Code == voucherCode && v.IsActive && v.ActiveFrom <= DateTime.Now && v.ActiveTo > DateTime.Now)
											.SingleOrDefaultAsync();

			if (voucher == null || price < voucher.DiscountThreshold)
				return price;

			if (voucher.DiscountPercentage != null)
				price -= (int)Math.Round(price * ((double)(voucher.DiscountPercentage ?? 0) / 100));
			else if (voucher.DiscountAmount != null)
				price -= voucher.DiscountAmount ?? 0;

			return price;
		}
	}
}
