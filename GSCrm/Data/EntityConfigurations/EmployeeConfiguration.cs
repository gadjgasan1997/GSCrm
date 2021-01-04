using GSCrm.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GSCrm.Data.EntityConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> modelBuilder)
        {
            modelBuilder
                .HasMany(empCon => empCon.EmployeeContacts)
                .WithOne(emp => emp.Empolyee)
                .HasForeignKey(empId => empId.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(empPos => empPos.EmployeePositions)
                .WithOne(emp => emp.Employee)
                .HasForeignKey(empId => empId.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(empResp => empResp.EmployeeResponsibilities)
                .WithOne(emp => emp.Employee)
                .HasForeignKey(empId => empId.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(accMan => accMan.AccountManagers)
                .WithOne(m => m.Manager)
                .HasForeignKey(manId => manId.ManagerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
