using finansije.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace finansije.Data
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPersonalInfo> UserPersonalInfos { get; set; }
        public DbSet<AddressInfo> AddressInfos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserPersonalInfo>().ToTable("UserPersonalInfo");
            modelBuilder.Entity<AddressInfo>().ToTable("AddressInfo");
            
            var userId = new Guid("11111111-1111-1111-1111-111111111111");

            var admin = new User
            {
                Id = userId,
                UserName = "superAdmin",
                Email = "admin@admin.com",
                Role = "admin",
                PasswordHash = "AQAAAAIAAYagAAAAEFyoa79LEYgh/11hp7jzsvPW6zqX4FMqNYMncb4P/LuxBd6hpP4HxZZ1dxBaaw7myA==",
                //password : admin123
                AddressInfoId = 1
            };

            modelBuilder.Entity<User>().HasData(admin);

            var adminInfo = new UserPersonalInfo
            {
                Id = 1,
                FirstName = "Nikola",
                LastName = "Nikolic",
                UserId = userId,
                DateOfBirth = new DateTime(1990, 1, 1),
                PhoneNumber = "061234578"
            };

            modelBuilder.Entity<UserPersonalInfo>().HasData(adminInfo);

            var adminAddressInfo = new AddressInfo
            {
                Id = 1,
                City = "Belgrade",
                Country = "Serbia",
                PostalCode = "104104",
                Street = "Bulevar Kralja Aleksandra",
                HouseNumber = 73
            };

            modelBuilder.Entity<AddressInfo>().HasData(adminAddressInfo);
        }
    }
}
