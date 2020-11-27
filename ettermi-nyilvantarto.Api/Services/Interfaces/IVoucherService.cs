using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IVoucherService
	{
		Task<PagedResult<VoucherListModel>> GetVouchers(int page);
		Task<AddResult> AddVoucher(VoucherAddModel model);
		Task ModifyVoucher(int id, VoucherModModel model);
		Task DeleteVoucher(int id);
	}
}
