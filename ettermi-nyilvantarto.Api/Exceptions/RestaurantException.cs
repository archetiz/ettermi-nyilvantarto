using System;

namespace ettermi_nyilvantarto.Api
{
	public abstract class RestaurantException : Exception
	{
		public abstract int StatusCode { get; set; }
		public RestaurantException()
		{
		}

		public RestaurantException(string message) : base(message)
		{
		}

		public RestaurantException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
