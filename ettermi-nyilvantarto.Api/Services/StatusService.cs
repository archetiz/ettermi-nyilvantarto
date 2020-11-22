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
		private readonly IDictionary<string, List<OrderSessionStatus>> statusVisibilities = new Dictionary<string, List<OrderSessionStatus>>()
		{
			{ Roles.Owner, new List<OrderSessionStatus>() { OrderSessionStatus.Active, OrderSessionStatus.Delivering, OrderSessionStatus.Paid, OrderSessionStatus.Cancelled } },
			{ Roles.Waiter, new List<OrderSessionStatus>() { OrderSessionStatus.Active, OrderSessionStatus.Delivering } },
			{ Roles.Chef, new List<OrderSessionStatus>() { OrderSessionStatus.Active } }
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
			statuses.ForEach(status => CheckStatusForRole(status, role));
		}

		public async Task CheckRightsForStatus(OrderSessionStatus status)
		{
			var role = await UserService.GetCurrentUserRole();
			CheckStatusForRole(status, role);
		}

		private void CheckStatusForRole(OrderSessionStatus status, string role)
		{
			if (!CanViewStatus(status, role))
				throw new RestaurantUnauthorizedException("Nincs jogosultsága a megadott állapotú rendelések megtekintéséhez!");
		}

		public bool CanViewStatus(OrderSessionStatus status, string role)
			=> statusVisibilities[role].Contains(status);
	}
}
