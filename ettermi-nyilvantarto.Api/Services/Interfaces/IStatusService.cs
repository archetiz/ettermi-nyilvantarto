using System;
using System.Collections.Generic;

namespace ettermi_nyilvantarto.Api
{
	public interface IStatusService
	{
		StatusType StringToStatus<StatusType>(string statusString) where StatusType : Enum;
		List<StatusType> GetStatusesFromList<StatusType>(List<string> statusesString) where StatusType : Enum;
	}
}
