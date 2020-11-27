using ettermi_nyilvantarto.Dbl.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IStatusService
	{
		StatusType StringToStatus<StatusType>(string statusString) where StatusType : Enum;
		List<StatusType> GetStatusesFromList<StatusType>(List<string> statusesString) where StatusType : Enum;
		Task CheckRightsForStatuses(List<OrderSessionStatus> statuses);
		Task CheckRightsForStatus(OrderSessionStatus sessionStatus, OrderStatus? orderStatus = null);
		bool CanViewStatus(string role, OrderSessionStatus sessionStatus, OrderStatus? orderStatus = null);
		Task CheckRightsForStatusModification(OrderSessionStatus sessionStatus, OrderStatus oldStatus, OrderStatus newStatus);
		Task CheckRightsForOrderAddDelete();
	}
}
