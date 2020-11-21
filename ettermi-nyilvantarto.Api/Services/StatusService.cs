using System;
using System.Collections.Generic;
using System.Linq;

namespace ettermi_nyilvantarto.Api
{
	public class StatusService : IStatusService
	{
		public StatusType StringToStatus<StatusType>(string statusString) where StatusType : Enum
			=> (StatusType)Enum.Parse(typeof(StatusType), statusString);

		public List<StatusType> GetStatusesFromList<StatusType>(List<string> statusesString) where StatusType : Enum
			=> statusesString.Select(statusString => StringToStatus<StatusType>(statusString)).ToList();
	}
}
