using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/invoice")]
	[Produces("application/json")]
	public class InvoiceController
	{
		private IInvoiceService InvoiceService { get; }
		public InvoiceController(IInvoiceService invoiceService)
		{
			this.InvoiceService = invoiceService;
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Owner,Waiter")]
		public async Task<FileStreamResult> GetInvoice(int id)
		{
			var stream = await InvoiceService.GetInvoice(id);
			return new FileStreamResult(stream, MediaTypeNames.Application.Pdf);
		}
	}
}
