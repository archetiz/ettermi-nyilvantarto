using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class CustomerService : ICustomerService
	{
		private RestaurantDbContext DbContext { get; }
		public CustomerService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<IEnumerable<CustomerListModel>> GetCustomers(string filter)
			=> (await DbContext.Customers
							.Where(c => c.IsActive && (string.IsNullOrEmpty(filter) || c.Name.Contains(filter) || c.PhoneNumber.Contains(filter) || c.Address.Contains(filter)))
							.OrderBy(c => c.Name)
							.ToListAsync())
								.Select(c => new CustomerListModel
								{
									Id = c.Id,
									Name = c.Name,
									PhoneNumber = c.PhoneNumber,
									Address = c.Address
								});

		public async Task<int> AddCustomer(CustomerAddModModel model)
		{
			var customer = DbContext.Customers.Add(new Customer()
			{
				Name = model.Name,
				PhoneNumber = model.PhoneNumber,
				Address = model.Address
			});

			await DbContext.SaveChangesAsync();

			return customer.Entity.Id;
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
