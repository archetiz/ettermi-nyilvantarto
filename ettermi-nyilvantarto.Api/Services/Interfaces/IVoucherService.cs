using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IVoucherService
	{
		Task<IEnumerable<VoucherListModel>> GetVouchers(int page);
		Task<int> AddVoucher(VoucherAddModel model);
		Task ModifyVoucher(int id, VoucherModModel model);
		Task DeleteVoucher(int id);
	}
}
