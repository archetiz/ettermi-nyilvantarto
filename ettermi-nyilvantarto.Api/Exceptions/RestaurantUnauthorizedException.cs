using System;

namespace ettermi_nyilvantarto.Api
{
	public class RestaurantUnauthorizedException : RestaurantException
	{
		public override int StatusCode { get; set; } = 401;
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
