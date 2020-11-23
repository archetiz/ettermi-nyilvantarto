using System;

namespace ettermi_nyilvantarto.Api
{
	class RestaurantInternalServerErrorException : RestaurantException
	{
		public override int StatusCode { get; set; } = 500;
		public RestaurantInternalServerErrorException()
		{
		}

		public RestaurantInternalServerErrorException(string message) : base(message)
		{
		}

		public RestaurantInternalServerErrorException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
