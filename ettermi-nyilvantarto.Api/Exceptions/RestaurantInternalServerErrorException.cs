using System;
using System.Net;

namespace ettermi_nyilvantarto.Api
{
	class RestaurantInternalServerErrorException : RestaurantException
	{
		public override int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
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
