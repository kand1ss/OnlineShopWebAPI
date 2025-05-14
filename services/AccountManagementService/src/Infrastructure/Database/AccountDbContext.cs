using AccountManagementService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementService.Infrastructure.Context;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
    }
}