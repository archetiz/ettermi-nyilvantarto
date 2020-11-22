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
		Task CheckRightsForStatus(OrderSessionStatus status);
		bool CanViewStatus(OrderSessionStatus status, string role);
	}
}
