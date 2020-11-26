using System;
using System.Net;

namespace ettermi_nyilvantarto.Api
{
	public class RestaurantBadRequestException : RestaurantException
	{
		public override int StatusCode { get; set; } = (int)HttpStatusCode.BadRequest;
		public RestaurantBadRequestException()
		{
		}

		public RestaurantBadRequestException(string message) : base(message)
		{
		}

		public RestaurantBadRequestException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
