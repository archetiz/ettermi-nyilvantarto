using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface ICustomerService
	{
		Task<PagedResult<CustomerListModel>> GetCustomers(string filter, int page);
		Task<AddResult> AddCustomer(CustomerAddModModel model);
		Task ModifyCustomer(int id, CustomerAddModModel model);
		Task DeleteCustomer(int id);
	}
}
