using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GSCrm.Data.EntityConfigurations;

namespace GSCrm.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountContact> AccountContacts { get; set; }
        public DbSet<AccountAddress> AccountAddresses { get; set; }
        public DbSet<AccountInvoice> AccountInvoices { get; set; }
        public DbSet<AccountQuote> AccountQuotes { get; set; }
        public DbSet<AccountManager> AccountManagers { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeContact> EmployeeContacts { get; set; }
        public DbSet<EmployeePosition> EmployeePositions { get; set; }
        public DbSet<EmployeeResponsibility> EmployeeResponsibilities { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }
        public DbSet<OrgNotificationsSetting> OrgNotificationsSettings { get; set; }
        public DbSet<UserNotificationsSetting> UserNotificationsSettings { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Responsibility> Responsibilities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InboxNotification> InboxNotifications { get; set; }

        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Database.SetCommandTimeout(180);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new AccountAddressConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteConfiguration());
            modelBuilder.ApplyConfiguration(new OrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }
}
