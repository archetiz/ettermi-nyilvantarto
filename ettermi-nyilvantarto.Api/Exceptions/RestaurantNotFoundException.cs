using System;

namespace ettermi_nyilvantarto.Api
{
	public class RestaurantNotFoundException : RestaurantException
	{
		public override int StatusCode { get; set; } = 404;
		public RestaurantNotFoundException()
		{
		}

		public RestaurantNotFoundException(string message) : base(message)
		{
		}

		public RestaurantNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
