using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using StoreClassLibrary;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace KLH60Manager.Data
{
    public partial class StoreManagerContext : IdentityDbContext
    {
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public IConfiguration Configuration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("StoreDB"));
            }
        }


        public StoreManagerContext() => Database.EnsureCreated();

        public StoreManagerContext(DbContextOptions<StoreManagerContext> options, IConfiguration config)
            : base(options)
        {
            Database.EnsureCreated();
            Configuration = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("KLam")
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItem");

                entity.HasIndex(e => e.CartId, "IX_CartItem_CartId");

                entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.ShoppingCart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId);
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

                entity.Property(e => e.Province)
                    .HasMaxLength(2);

                entity.HasOne(d => d.ShoppingCart).WithOne(p => p.Customer)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.Orders).WithOne(p => p.Customer).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasIndex(e => e.OrderCustId, "IX_Order_OrderCustId");

                entity.Property(e => e.Taxes).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderCustId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItem");

                entity.HasIndex(e => e.OrderId, "IX_OrderItem_OrderId");

                entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

                entity.HasOne(d => d.Orders)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasIndex(e => e.ProdCatId, "IX_Product_ProdCatId");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.BuyPrice).HasColumnType("numeric(8, 2)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Manufacturer)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SellPrice).HasColumnType("numeric(8, 2)");

                entity.HasOne(d => d.ProdCat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProdCatId)
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

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.CartId);

                entity.ToTable("ShoppingCart");

                entity.HasIndex(e => e.CartCustId, "IX_ShoppingCart_CartCustId")
                    .IsUnique();

                entity.HasOne(d => d.Customer)
                    .WithOne(p => p.ShoppingCart)
                    .HasForeignKey<ShoppingCart>(d => d.CartCustId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}