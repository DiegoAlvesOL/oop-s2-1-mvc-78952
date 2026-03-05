using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Entities;

namespace Library.mvc.Data;


public class LibraryDbContext : IdentityDbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
 
        base.OnModelCreating(modelBuilder);

 
        modelBuilder.Entity<Loan>()
            .HasOne(loan => loan.Book)
            .WithMany(book => book.Loans)
            .HasForeignKey(loan => loan.BookId);

 
        modelBuilder.Entity<Loan>()
            .HasOne(loan => loan.Member)
            .WithMany(member => member.Loans)
            .HasForeignKey(loan => loan.MemberId);
    }
}