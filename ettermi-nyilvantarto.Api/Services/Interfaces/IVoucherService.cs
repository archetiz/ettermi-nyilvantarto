﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IVoucherService
	{
		Task<IEnumerable<VoucherListModel>> GetVouchers();
		Task<int> AddVoucher(VoucherAddModel model);
		Task DeleteVoucher(int id);
	}
}