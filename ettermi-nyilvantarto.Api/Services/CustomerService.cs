using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class CustomerService : ICustomerService
	{
		private RestaurantDbContext DbContext { get; }
		private PagingConfiguration PagingConfig { get; }
		public CustomerService(RestaurantDbContext dbContext, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<PagedResult<CustomerListModel>> GetCustomers(string filter, int page)
			=> (await DbContext.Customers
							.Where(c => c.IsActive && (string.IsNullOrEmpty(filter) || c.Name.Contains(filter) || c.PhoneNumber.Contains(filter) || c.Address.Contains(filter)))
							.OrderBy(c => c.Name)
							.GetPaged(page, PagingConfig.PageSize, out int totalPages)
							.Select(c => new CustomerListModel
							{
								Id = c.Id,
								Name = c.Name,
								PhoneNumber = c.PhoneNumber,
								Address = c.Address
							}).ToListAsync()).GetPagedResult(page, PagingConfig.PageSize, totalPages);

		public async Task<AddResult> AddCustomer(CustomerAddModModel model)
		{
			var customer = DbContext.Customers.Add(new Customer()
			{
				Name = model.Name,
				PhoneNumber = model.PhoneNumber,
				Address = model.Address
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(customer.Entity.Id);
		}

		public async Task ModifyCustomer(int id, CustomerAddModModel model)
		{
			var customer = await DbContext.Customers.FindAsync(id);

			if (customer == null)
				throw new RestaurantNotFoundException("Nem létező vendég!");

			customer.Name = model.Name ?? customer.Name;
			customer.PhoneNumber = model.PhoneNumber ?? customer.PhoneNumber;
			customer.Address = model.Address ?? customer.Address;

			await DbContext.SaveChangesAsync();
		}

		public async Task DeleteCustomer(int id)
		{
			var customer = await DbContext.Customers.FindAsync(id);

			if (customer == null)
				throw new RestaurantNotFoundException("Nem létező vendég!");

			customer.IsActive = false;
			customer.Name = "Törölt vendég";
			customer.PhoneNumber = "X";
			customer.Address = "X";

			await DbContext.SaveChangesAsync();
		}
	}
}
