using System;
using System.Net;

namespace ettermi_nyilvantarto.Api
{
	public class RestaurantUnauthorizedException : RestaurantException
	{
		public override int StatusCode { get; set; } = (int)HttpStatusCode.Unauthorized;
		public RestaurantUnauthorizedException()
		{
		}

		public RestaurantUnauthorizedException(string message) : base(message)
		{
		}

		public RestaurantUnauthorizedException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
