﻿using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class OrderSessionService : IOrderSessionService
	{
		private RestaurantDbContext DbContext { get; }
		public IUserService UserService { get; }
		public IStatusService StatusService { get; }

		public OrderSessionService(RestaurantDbContext dbContext, IUserService userService, IStatusService statusService)
		{
			DbContext = dbContext;
			UserService = userService;
			StatusService = statusService;
		}

		public async Task<IEnumerable<OrderSessionListModel>> GetOrderSessions(List<string> statusStrings)
		{
			var statuses = StatusService.GetStatusesFromList<OrderSessionStatus>(statusStrings);

			await CheckRightsForStatuses(statuses);

			return (await DbContext.OrderSessions.Where(os => statuses.Contains(os.Status) || statuses.Count() == 0).ToListAsync())
				.Select(os => new OrderSessionListModel()
				{
					Id = os.Id,
					TableId = os.TableId,
					CustomerId = os.CustomerId,
					VoucherId = os.VoucherId,
					InvoiceId = os.InvoiceId,
					Status = os.Status
				});
		}

		public async Task<OrderSessionDataModel> GetOrderSessionDetails(int id)
		{
			var orderSession = await DbContext.OrderSessions
										.Include(os => os.Customer)
										.Include(os => os.Voucher)
										.Include(os => os.Table)
										.Include(os => os.Orders)
											.Where(os => os.Id == id).SingleOrDefaultAsync();

			if (orderSession == null)
				throw new RestaurantNotFoundException("Nem létező rendelési folyamat!");

			await CheckRightsForStatus(orderSession.Status);

			var orders = new List<OrderListModel>();
			orderSession.Orders.ForEach(order =>
			{
				orders.Add(new OrderListModel()
				{
					Id = order.Id,
					WaiterId = order.WaiterUserId,
					Status = (int)order.Status
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
				Status = (int)orderSession.Status,
				Orders = orders
			};
		}

		public async Task ModifyOrderSessionStatus(int id, StatusModModel model)
		{
			var orderSession = await DbContext.OrderSessions.FindAsync(id);

			if (orderSession == null)
				throw new RestaurantNotFoundException("Nem létező rendelési folyamat!");

			await CheckRightsForStatus(orderSession.Status);

			orderSession.Status = StatusService.StringToStatus<OrderSessionStatus>(model.Status);

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
															.Where(os => os.Id == id).SingleOrDefaultAsync();

			if (orderSession == null)
				throw new RestaurantNotFoundException("Nem létező rendelési folyamat!");

			//Calculate base price
			var price = CalculatePrice(orderSession);

			//Note: calculation order matters here, we redeem loyalty points 1st, because if the vouchers are percentage based, overall this results in potentially higher net gain for the restaurant

			//Loyalty points
			if (model.LoyaltyCardNumber != null)
			{
				var loyaltyCard = await DbContext.LoyaltyCards.Where(lc => lc.CardNumber == model.LoyaltyCardNumber).SingleOrDefaultAsync();

				if (loyaltyCard == null)
					throw new RestaurantNotFoundException("Nem létező hűségkártya!");

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
				loyaltyCard.Points += 21;                                               //MOCK
			}

			//Vouchers
			if (!string.IsNullOrEmpty(model.VoucherCode))
				price = await CalculateVoucherDiscountedPrice(model.VoucherCode, price);

			//Close order
			orderSession.Status = OrderSessionStatus.Paid;

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
				order.Items.ForEach(oi =>
				{
					sum += oi.Quantity * oi.MenuItem.Price;
				});
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