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
		private IVoucherService VoucherService { get; }
		public VoucherController(IVoucherService voucherService)
		{
			this.VoucherService = voucherService;
		}

		[HttpGet]
		[HttpGet("page/{page}")]
		public async Task<IEnumerable<VoucherListModel>> ListVouchers(int page = 1)
			=> await VoucherService.GetVouchers(page);

		[HttpPost]
		public async Task<int> AddVoucher(VoucherAddModel voucher)
			=> await VoucherService.AddVoucher(voucher);

		[HttpPut("{id}")]
		public async Task ModifyVoucher(int id, VoucherModModel voucher)
				=> await VoucherService.ModifyVoucher(id, voucher);

		[HttpDelete("{id}")]
		public async Task DeleteVoucher(int id)
			=> await VoucherService.DeleteVoucher(id);
	}
}
