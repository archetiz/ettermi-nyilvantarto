using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/customer")]
	[Produces("application/json")]
	[Authorize(Roles = "Owner,Waiter")]
	public class CustomerController
	{
		private ICustomerService CustomerService { get; }
		public CustomerController(ICustomerService customerService)
		{
			this.CustomerService = customerService;
		}

		[HttpGet]
		public async Task<IEnumerable<CustomerListModel>> GetCustomers([FromQuery] string query)
			=> await CustomerService.GetCustomers(query);

		[HttpPost]
		public async Task<int> AddCustomer(CustomerAddModModel customer)
			=> await CustomerService.AddCustomer(customer);

		[HttpPut("{id}")]
		public async Task ModifyCustomer(int id, CustomerAddModModel customer)
			=> await CustomerService.ModifyCustomer(id, customer);

		[HttpDelete("{id}")]
		public async Task DeleteCustomer(int id)
			=> await CustomerService.DeleteCustomer(id);
	}
}
