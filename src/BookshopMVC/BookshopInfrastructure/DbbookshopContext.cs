using System;
using System.Collections.Generic;
using BookshopDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace BookshopInfrastructure;

public partial class DbbookshopContext : DbContext
{
    public DbbookshopContext()
    {
        Database.EnsureCreated();
    }

    public DbbookshopContext(DbContextOptions<DbbookshopContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrdersBook> OrdersBooks { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HOME-PC\\SQLEXPRESS; Database=DBBookshop; Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(e => e.Address1)
                .HasColumnType("ntext")
                .HasColumnName("Address");

            entity.HasOne(d => d.Country).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ADDRESSES_COUNTRIES");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.FullName)
            .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "BooksAuthor",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_BooksAuthors_Books"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_BOOKSAUTHORS_AUTHORS"),
                    j =>
                    {
                        j.HasKey("AuthorId", "BookId").HasName("PK_BooksAuthors_1");
                        j.ToTable("BooksAuthors");
                    });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_BOOKS_PUBLISHERS");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Genres");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Books).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "BooksCategory",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_BooksCategories_Books"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("FK_BooksCategories_Categories"),
                    j =>
                    {
                        j.HasKey("CategoryId", "BookId");
                        j.ToTable("BooksCategories");
                    });
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_ADRESSES");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_USERS");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_PAYMENT_METHODS");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_STATUSES");
        });

        modelBuilder.Entity<OrdersBook>(entity =>
        {
            entity.HasOne(d => d.Book).WithMany(p => p.OrdersBooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERSBOOKS_BOOKID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersBooks)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERSBOOKS_ORDER");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
