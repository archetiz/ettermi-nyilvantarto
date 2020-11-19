using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ettermi_nyilvantarto.Dbl
{
	public class RestaurantDbContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Feedback> Feedback { get; set; }
		public DbSet<LoyaltyCard> LoyaltyCards { get; set; }
		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<Table> Tables { get; set; }
		public DbSet<Voucher> Vouchers { get; set; }

		public RestaurantDbContext(DbContextOptions options) : base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
		}
	}
}
