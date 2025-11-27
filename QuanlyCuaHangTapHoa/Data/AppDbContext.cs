using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Models;
using System.IO;
using Microsoft.Maui.Storage;

namespace QuanlyCuaHangTapHoa.Data
{
    /// <summary>
    /// DbContext chính của ứng dụng, làm việc với SQLite
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Fallback khi context không được cấu hình qua DI.
        /// Bình thường app sẽ dùng cấu hình trong MauiProgram.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "grocery_store.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Quan hệ 1-n: Category - Products
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: Supplier - PurchaseOrders
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: User - PurchaseOrders
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.CreatedByUser)
                .WithMany()
                .HasForeignKey(po => po.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: PurchaseOrder - Details
            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(d => d.PurchaseOrder)
                .WithMany(po => po.Details)
                .HasForeignKey(d => d.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n: Sale - Details
            modelBuilder.Entity<SaleDetail>()
                .HasOne(d => d.Sale)
                .WithMany(s => s.Details)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n: Product - SaleDetails
            modelBuilder.Entity<SaleDetail>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: Product - PurchaseOrderDetails
            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: Customer - Sales
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ 1-n: User - Sales
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ 1-n: Product - StockMovements
            modelBuilder.Entity<StockMovement>()
                .HasOne(sm => sm.Product)
                .WithMany()
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ 1-n: User - LoginHistories
            modelBuilder.Entity<LoginHistory>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Seed data cơ bản (admin + vài category & product demo) =====
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1);

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "Admin@123", // TẠM THỜI: nhớ đồng bộ với logic Auth
                Role = "Admin",
                FullName = "Quản trị hệ thống",
                Email = "admin@example.com",
                Phone = "0123456789",
                CreatedAt = seedDate,
                IsActive = true
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Đồ uống", IsActive = true },
                new Category { Id = 2, Name = "Bánh kẹo", IsActive = true },
                new Category { Id = 3, Name = "Gia vị", IsActive = true },
                new Category { Id = 4, Name = "Nhu yếu phẩm", IsActive = true },
                new Category { Id = 5, Name = "Đồ hộp", IsActive = true }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Code = "NUOCNGOT001",
                    Name = "Coca Cola lon",
                    CategoryId = 1,
                    Unit = "Lon",
                    PurchasePrice = 7000,
                    SellingPrice = 10000,
                    StockQuantity = 50,
                    Status = "InStock",
                    CreatedAt = seedDate,
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Code = "NUOCNGOT002",
                    Name = "Pepsi lon",
                    CategoryId = 1,
                    Unit = "Lon",
                    PurchasePrice = 7000,
                    SellingPrice = 10000,
                    StockQuantity = 40,
                    Status = "InStock",
                    CreatedAt = seedDate,
                    IsActive = true
                },
                new Product
                {
                    Id = 3,
                    Code = "BANH001",
                    Name = "Bánh Oreo",
                    CategoryId = 2,
                    Unit = "Gói",
                    PurchasePrice = 6000,
                    SellingPrice = 9000,
                    StockQuantity = 30,
                    Status = "InStock",
                    CreatedAt = seedDate,
                    IsActive = true
                }
            );
        }
    }
}
