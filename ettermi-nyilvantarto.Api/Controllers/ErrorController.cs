using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ettermi_nyilvantarto.Api.Controllers
{
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorController : ControllerBase
	{
		[Route("error")]
		public ErrorModel HandleError()
		{
			var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
			var exception = context?.Error;

			var error = new ErrorModel();

			if (exception is RestaurantException restaurantException)
			{
				Response.StatusCode = restaurantException.StatusCode;
				error.ResultError = restaurantException.Message;
			}
			else
			{
				Response.StatusCode = 500;
				error.ResultError = "Nem várt hiba történt!";
			}
			return error;
		}
	}
}
