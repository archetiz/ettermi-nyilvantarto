using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class StatusService : IStatusService
	{
		//Status visibilities by role
		private readonly IDictionary<string, List<OrderSessionStatus>> sessionStatusVisibilities = new Dictionary<string, List<OrderSessionStatus>>()
		{
			{ Roles.Owner, new List<OrderSessionStatus>() { OrderSessionStatus.Active, OrderSessionStatus.Delivering, OrderSessionStatus.Paid, OrderSessionStatus.Cancelled } },
			{ Roles.Waiter, new List<OrderSessionStatus>() { OrderSessionStatus.Active, OrderSessionStatus.Delivering } },
			{ Roles.Chef, new List<OrderSessionStatus>() { OrderSessionStatus.Active } }
		};
		private readonly IDictionary<string, List<OrderStatus>> orderStatusVisibilities = new Dictionary<string, List<OrderStatus>>()
		{
			{ Roles.Owner, new List<OrderStatus>() { OrderStatus.Ordering, OrderStatus.Ordered, OrderStatus.Preparing, OrderStatus.Prepared, OrderStatus.Served, OrderStatus.Cancelled } },
			{ Roles.Waiter, new List<OrderStatus>() { OrderStatus.Ordering, OrderStatus.Ordered, OrderStatus.Preparing, OrderStatus.Prepared, OrderStatus.Served, OrderStatus.Cancelled } },
			{ Roles.Chef, new List<OrderStatus>() { OrderStatus.Ordered, OrderStatus.Preparing, OrderStatus.Prepared, OrderStatus.Cancelled } }
		};
		//--

		private IUserService UserService { get; }
		public StatusService(IUserService userService)
		{
			this.UserService = userService;
		}

		public StatusType StringToStatus<StatusType>(string statusString) where StatusType : Enum
			=> (StatusType)Enum.Parse(typeof(StatusType), statusString);

		public List<StatusType> GetStatusesFromList<StatusType>(List<string> statusesString) where StatusType : Enum
			=> statusesString.Select(statusString => StringToStatus<StatusType>(statusString)).ToList();

		public async Task CheckRightsForStatuses(List<OrderSessionStatus> statuses)
		{
			var role = await UserService.GetCurrentUserRole();
			statuses.ForEach(status => CheckStatusForRole(role, status));
		}

		public async Task CheckRightsForStatus(OrderSessionStatus sessionStatus, OrderStatus? orderStatus = null)
		{
			var role = await UserService.GetCurrentUserRole();
			CheckStatusForRole(role, sessionStatus, orderStatus);
		}

		private void CheckStatusForRole(string role, OrderSessionStatus sessionStatus, OrderStatus? orderStatus = null)
		{
			if (!CanViewStatus(role, sessionStatus, orderStatus))
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		}

		public bool CanViewStatus(string role, OrderSessionStatus sessionStatus, OrderStatus? orderStatus = null)
			=> sessionStatusVisibilities[role].Contains(sessionStatus) && (orderStatus == null || orderStatusVisibilities[role].Contains(orderStatus ?? 0));

		public async Task CheckRightsForStatusModification(OrderSessionStatus sessionStatus, OrderStatus oldStatus, OrderStatus newStatus)
		{
			var role = await UserService.GetCurrentUserRole();
			
			if (role != Roles.Owner && oldStatus == OrderStatus.Cancelled)
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a rendelés módosításához!");

			if (role == Roles.Chef && (newStatus == OrderStatus.Served || newStatus == OrderStatus.Cancelled || oldStatus == OrderStatus.Ordering || newStatus == OrderStatus.Ordering))
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a kért művelet végrehajtásához!");

			CheckStatusForRole(role, sessionStatus, oldStatus);
		}

		public async Task CheckRightsForOrderAddDelete()
		{
			var role = await UserService.GetCurrentUserRole();
			if (role == Roles.Chef)
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a kért művelethez!");
		}
	}
}
