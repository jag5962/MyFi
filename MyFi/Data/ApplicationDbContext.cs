using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFi.Data.Models;

namespace MyFi.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Expense>().Property(o => o.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<RecurringExpense>().Property(o => o.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<RecurringExpenseDefault>().Property(o => o.DefaultAmount).HasPrecision(12, 2);
        modelBuilder.Entity<Savings>().Property(o => o.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<SavingsDefault>().Property(o => o.DefaultAmount).HasPrecision(12, 2);
        modelBuilder.Entity<Pay>().Property(o => o.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<Income>().Property(o => o.DefaultPay).HasPrecision(12, 2);
        modelBuilder.Entity<Income>().Property(o => o.AnnualGrossPay).HasPrecision(12, 2);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
    public DbSet<RecurringExpense> RecurringExpenses => Set<RecurringExpense>();
    public DbSet<RecurringExpenseDefault> RecurringExpenseDefaults => Set<RecurringExpenseDefault>();
    public DbSet<RecurringExpenseType> RecurringExpenseTypes => Set<RecurringExpenseType>();
    public DbSet<Savings> Savings => Set<Savings>();
    public DbSet<SavingsDefault> SavingsDefaults => Set<SavingsDefault>();
    public DbSet<SavingsType> SavingsTypes => Set<SavingsType>();
    public DbSet<Pay> Pays => Set<Pay>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<IncomeType> IncomeTypes => Set<IncomeType>();
}
