using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StoreClassLibrary;
using System.Data;

#nullable disable

namespace KLH60Store.Models
{
    public partial class StoreContext : IdentityDbContext
    {
        public virtual DbSet<CartItem> CartItem { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCart { get; set; }
        public IConfiguration Configuration { get; }

        public StoreContext()
        {
        }

        public StoreContext(IConfiguration config) => Configuration = config;

        public StoreContext(DbContextOptions<StoreContext> options) : base(options) => Database.EnsureCreated();

        public StoreContext(DbContextOptions<StoreContext> options, IConfiguration config) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception e)
            {
                //TODO: FIND HOW TO CATCH THIS EXCEPTION WHEN NOT CONNECTED TO VPN
            }
            Configuration = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("StoreDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("KLam").HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.BuyPrice).HasColumnType("numeric(8, 2)");

                entity.Property(e => e.Description)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.Manufacturer)
                    .HasMaxLength(80)
                    .IsUnicode(false);

                entity.Property(e => e.SellPrice).HasColumnType("numeric(8, 2)");

                entity.HasOne(d => d.ProdCat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProdCatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductCategory");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.ToTable("ProductCategory");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ProdCat)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.HasIndex(e => e.AspnetUserId, "IX_Customer_AspnetUserId");

                entity.Property(e => e.CreditCard)
                    .HasMaxLength(16)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.Email).HasMaxLength(30);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.LastName)
                    .HasMaxLength(20)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasDefaultValueSql("(N'')");

                entity.Property(e => e.Province).HasMaxLength(2);

                entity.HasOne(d => d.ShoppingCart)
                    .WithOne(p => p.Customer)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.Orders).WithOne(p => p.Customer).OnDelete
                (DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Customer>().HasData(
                new Customer(1, "Goose", "Man", "an@email.com", "8197734608", "QC", "5299014012611141"),
                new Customer(2, "Dat", "Boi", "dat@boi.com", "8193034847", "ON", "5384861194865788"),
                new Customer(3, "Kana", "Kamiko", "kk@gmail.com", "8196734682", "QC", "5130403899507394")
            );

            modelBuilder.Entity<Order>().HasData(
                new Order() { OrderId = 1, OrderCustId = 3, DateCreated = new DateTime(2020, 8, 17), DateFulfiled = new DateTime(2020, 8, 7), Taxes = 28.5M, Total = 218.5M });

            modelBuilder.Entity<ShoppingCart>().HasData(
                new ShoppingCart() { CartId = 1, CartCustId = 2, DateCreated = new DateTime(2019, 5, 20) });

            modelBuilder.Entity<CartItem>().HasData(
                new CartItem() { CartItemId = 1, Quantity = 1, Price = 105.35M, CartId = 1, ProductId = 7 },
                new CartItem() { CartItemId = 2, Quantity = 3, Price = 17.97M, CartId = 1, ProductId = 13 },
                new CartItem() { CartItemId = 3, Quantity = 1, Price = 89.99M, CartId = 1, ProductId = 22 });

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem() { OrderItemId = 1, Quantity = 1, Price = 85.25M, OrderId = 1, ProductId = 3 },
                new OrderItem() { OrderItemId = 2, Quantity = 1, Price = 67.85M, OrderId = 1, ProductId = 6 },
                new OrderItem() { OrderItemId = 3, Quantity = 1, Price = 77.99M, OrderId = 1, ProductId = 19 });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}