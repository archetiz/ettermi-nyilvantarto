using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/voucher")]
	[Produces("application/json")]
	[Authorize(Roles = "Owner")]
	public class VoucherController : ControllerBase
	{
		private IVoucherService VoucherService { get; set; }
		public VoucherController(IVoucherService voucherService)
		{
			this.VoucherService = voucherService;
		}

		[HttpGet]
		public async Task<IEnumerable<VoucherListModel>> ListVouchers()
			=> await VoucherService.GetVouchers();

		[HttpPost]
		public async Task<int> AddVoucher(VoucherAddModel voucher)
			=> await VoucherService.AddVoucher(voucher);

		[HttpDelete("{id}")]
		public async Task DeleteVoucher(int id)
			=> await VoucherService.DeleteVoucher(id);
	}
}
