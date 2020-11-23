using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ICustomerService
	{
		Task<IEnumerable<CustomerListModel>> GetCustomers(string filter, int page);
		Task<int> AddCustomer(CustomerAddModModel model);
		Task ModifyCustomer(int id, CustomerAddModModel model);
		Task DeleteCustomer(int id);
	}
}
