using ettermi_nyilvantarto.Dbl.Entities;
using ettermi_nyilvantarto.Dbl.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ettermi_nyilvantarto.Dbl
{
	public class RestaurantDbContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Feedback> Feedback { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<LoyaltyCard> LoyaltyCards { get; set; }
		public DbSet<MenuItem> MenuItems { get; set; }
		public DbSet<MenuItemCategory> MenuItemCategories { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderSession> OrderSessions { get; set; }
		public DbSet<Reservation> Reservations { get; set; }
		public DbSet<Table> Tables { get; set; }
		public DbSet<TableCategory> TableCategories { get; set; }
		public DbSet<Voucher> Vouchers { get; set; }

		public RestaurantDbContext(DbContextOptions options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			CreateTables(modelBuilder);
			ApplyLogicalConstraints(modelBuilder);
			ApplyConnections(modelBuilder);
			SeedData(modelBuilder);
		}

		private void CreateTables(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
		}

		private void ApplyLogicalConstraints(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Customer>().Property("PhoneNumber").HasMaxLength(15);

			modelBuilder.Entity<Feedback>().HasCheckConstraint("CK_FeedbackRating", "Rating >= 0 AND Rating <= 5");
			modelBuilder.Entity<Feedback>().Property("Date").IsRequired();

			modelBuilder.Entity<LoyaltyCard>().Property("CardNumber").IsRequired();
			modelBuilder.Entity<LoyaltyCard>().HasIndex("CardNumber").IsUnique();

			modelBuilder.Entity<MenuItem>().Property("Name").IsRequired();
			modelBuilder.Entity<MenuItem>().HasIndex("Name").IsUnique();

			modelBuilder.Entity<Order>().Property("Status").IsRequired();

			modelBuilder.Entity<OrderSession>().Property("Status").IsRequired();

			modelBuilder.Entity<Reservation>().HasCheckConstraint("CK_ReservationDates", "TimeFrom < TimeTo");

			modelBuilder.Entity<Table>().Property("Code").IsRequired();
			modelBuilder.Entity<Table>().HasIndex("Code").IsUnique();

			modelBuilder.Entity<Voucher>().Property("Code").IsRequired();
			modelBuilder.Entity<Voucher>().HasIndex("Code").IsUnique();
			modelBuilder.Entity<Voucher>().HasCheckConstraint("CK_VoucherDates", "ActiveFrom < ActiveTo");
		}

		private void ApplyConnections(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<MenuItem>().HasOne(mi => mi.Category).WithMany(mic => mic.MenuItems);
			modelBuilder.Entity<Table>().HasOne(t => t.Category).WithMany(tc => tc.Tables);
			modelBuilder.Entity<Order>().HasMany(o => o.Items);
			modelBuilder.Entity<OrderSession>().HasMany(os => os.Orders).WithOne(o => o.OrderSession);
			modelBuilder.Entity<OrderSession>().HasOne(os => os.Invoice).WithOne(i => i.OrderSession).HasForeignKey<Invoice>(i => i.OrderSessionId);
		}

		private void SeedData(ModelBuilder modelBuilder)
		{
			TableCategoriesDataSeed tableCategoriesDataSeed = new TableCategoriesDataSeed();
			MenuItemCategoriesDataSeed menuItemCategoriesDataSeed = new MenuItemCategoriesDataSeed();

			modelBuilder.Entity<MenuItemCategory>().HasData(menuItemCategoriesDataSeed.MenuItemCategories);
			modelBuilder.Entity<TableCategory>().HasData(tableCategoriesDataSeed.TableCategories);
		}
	}
}
