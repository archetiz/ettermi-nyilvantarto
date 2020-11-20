using System;

namespace ettermi_nyilvantarto.Api
{
	public class RestaurantBadRequestException : RestaurantException
	{
		public override int StatusCode { get; set; } = 500;
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
