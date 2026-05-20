using HandMade.Domain.Entities;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace HandMade.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityAppUser, IdentityAppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<ShopFollower> ShopFollowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                  .LogTo(log => Debug.WriteLine(log), Microsoft.Extensions.Logging.LogLevel.Information)
                  .EnableSensitiveDataLogging(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ──────────────────────────────────────────────
            // IdentityAppUser — owns ALL relationships that
            // touch the user table. Other entities use only
            // the scalar FK (UserId) and no back-nav prop.
            // ──────────────────────────────────────────────
            modelBuilder.Entity<IdentityAppUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();

                // Cart — one-to-one; back-nav removed from Cart
                entity.HasOne(u => u.Cart)
                      .WithOne()
                      .HasForeignKey<Cart>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // CartItems
                entity.HasMany(u => u.CartItems)
                      .WithOne()
                      .HasForeignKey(ci => ci.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Shops
                entity.HasMany(u => u.Shops)
                      .WithOne()
                      .HasForeignKey(s => s.OwnerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Orders
                entity.HasMany(u => u.Orders)
                      .WithOne()
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Payments
                entity.HasMany(u => u.Payments)
                      .WithOne()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Favorites
                entity.HasMany(u => u.Favorites)
                      .WithOne()
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // UserNotifications
                entity.HasMany(u => u.UserNotifications)
                      .WithOne()
                      .HasForeignKey(un => un.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Disputes
                entity.HasMany(u => u.Disputes)
                      .WithOne()
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ShopFollowers
                entity.HasMany(u => u.ShopFollowers)
                      .WithOne()
                      .HasForeignKey(sf => sf.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Reviews written by this user (as reviewer)
                entity.HasMany(u => u.WrittenReviews)
                      .WithOne()
                      .HasForeignKey(r => r.ReviewerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Reviews received by this user (as reviewed buyer)
                // TargetId is a shared FK across Product/Shop/User reviews —
                // no HasForeignKey here; EF resolves it via the Review config below.
                entity.HasMany(u => u.ReceivedReviews)
                      .WithOne()
                      .HasForeignKey(r => r.TargetId)
                      .HasPrincipalKey(u => u.Id)
                      .OnDelete(DeleteBehavior.Restrict);

                // Addresses — UserId nullable (SetNull on delete)
                entity.HasMany<Address>()
                      .WithOne()
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // ──────────────────────────────────────────────
            // Shop
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Shop>(entity =>
            {
                // Owner relationship is already configured from IdentityAppUser side above.
                // Only non-user relationships live here.

                entity.Property(s => s.RatingAverage).HasPrecision(3, 2);
            });

            // ──────────────────────────────────────────────
            // Category
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name).IsUnique();
            });

            // ──────────────────────────────────────────────
            // SubCategory
            // ──────────────────────────────────────────────
            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.HasOne(sc => sc.Category)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(sc => sc.CategoryId);

                entity.HasIndex(sc => new { sc.CategoryId, sc.Name }).IsUnique();
            });

            // ──────────────────────────────────────────────
            // Product
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasQueryFilter(p => !p.IsDeleted);

                entity.HasOne(p => p.Shop)
                      .WithMany(s => s.Products)
                      .HasForeignKey(p => p.ShopId);

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.SubCategory)
                      .WithMany(sc => sc.Products)
                      .HasForeignKey(p => p.SubCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(p => new { p.ShopId, p.SKU }).IsUnique();

                entity.Property(p => p.Price).HasPrecision(18, 2);
            });

            // ──────────────────────────────────────────────
            // ProductImage
            // ──────────────────────────────────────────────
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(pi => pi.ProductId);
            });

            // ──────────────────────────────────────────────
            // Cart
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Cart>(entity =>
            {
                // Relationship with IdentityAppUser is configured from the
                // IdentityAppUser block above — do NOT repeat it here.

                entity.HasMany(c => c.CartItems)
                      .WithOne(ci => ci.Cart)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ──────────────────────────────────────────────
            // CartItem
            // ──────────────────────────────────────────────
            modelBuilder.Entity<CartItem>(entity =>
            {
                // Cart side of the relationship is already configured above.

                entity.HasOne(ci => ci.Product)
                      .WithMany(p => p.CartItems)
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();

                entity.Property(ci => ci.UnitPrice).HasPrecision(18, 2);
                entity.Property(ci => ci.TotalPrice).HasPrecision(18, 2);
            });

            // ──────────────────────────────────────────────
            // Address
            // ──────────────────────────────────────────────
            // User relationship configured from IdentityAppUser block above.
            // Only non-user relationships live here (none currently).

            // ──────────────────────────────────────────────
            // Order
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Order>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(o => o.Product)
                      .WithMany()
                      .HasForeignKey(o => o.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Shop)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(o => o.ShopId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.ShippingAddress)
                      .WithMany(a => a.Orders)
                      .HasForeignKey(o => o.ShippingAddressId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.OrderNumber).IsUnique();

                entity.Property(o => o.Subtotal).HasPrecision(18, 2);
                entity.Property(o => o.ShippingFee).HasPrecision(18, 2);
                entity.Property(o => o.TaxTotal).HasPrecision(18, 2);
                entity.Property(o => o.GrandTotal).HasPrecision(18, 2);
            });

            // ──────────────────────────────────────────────
            // Payment
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Payment>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(p => p.Order)
                      .WithMany(o => o.Payments)
                      .HasForeignKey(p => p.OrderId);

                entity.Property(p => p.Amount).HasPrecision(18, 2);
            });

            // ──────────────────────────────────────────────
            // Refund
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Refund>(entity =>
            {
                entity.HasOne(r => r.Order)
                      .WithMany(o => o.Refunds)
                      .HasForeignKey(r => r.OrderId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Payment)
                      .WithMany(p => p.Refunds)
                      .HasForeignKey(r => r.PaymentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.Amount).HasPrecision(18, 2);
            });

            // ──────────────────────────────────────────────
            // Shipment
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasOne(s => s.Order)
                      .WithMany(o => o.Shipments)
                      .HasForeignKey(s => s.OrderId);

                entity.HasOne(s => s.ShippingAddress)
                      .WithMany(a => a.Shipments)
                      .HasForeignKey(s => s.ShippingAddressId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ──────────────────────────────────────────────
            // Review
            // ──────────────────────────────────────────────
            // IMPORTANT: TargetId is a shared FK used for three review targets
            // (Product, Shop, reviewed Buyer). EF cannot enforce all three as
            // true FKs simultaneously — use discriminator filtering in queries
            // (WHERE TargetType = 'Product') and mark each HasForeignKey as
            // IsRequired(false) so EF doesn't enforce DB-level referential
            // integrity across all three at once.
            modelBuilder.Entity<Review>(entity =>
            {
                // Reviewer — configured from IdentityAppUser block above (WrittenReviews).
                // ReceivedReviews (reviewed buyer) — also configured from IdentityAppUser block.

                entity.HasOne(r => r.Product)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(r => r.TargetId)
                      .HasPrincipalKey(p => p.Id)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasOne(r => r.Shop)
                      .WithMany(s => s.Reviews)
                      .HasForeignKey(r => r.TargetId)
                      .HasPrincipalKey(s => s.Id)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                entity.HasIndex(r => new { r.ReviewerUserId, r.TargetType, r.TargetId })
                      .IsUnique();
            });

            // ──────────────────────────────────────────────
            // Favorite
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Favorite>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(f => f.Product)
                      .WithMany(p => p.Favorites)
                      .HasForeignKey(f => f.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(f => new { f.UserId, f.ProductId }).IsUnique();
            });

            // ──────────────────────────────────────────────
            // UserNotification
            // ──────────────────────────────────────────────
            modelBuilder.Entity<UserNotification>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(un => un.Notification)
                      .WithMany(n => n.UserNotifications)
                      .HasForeignKey(un => un.NotificationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ──────────────────────────────────────────────
            // Dispute
            // ──────────────────────────────────────────────
            modelBuilder.Entity<Dispute>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(d => d.Order)
                      .WithMany(o => o.Disputes)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ──────────────────────────────────────────────
            // ShopFollower
            // ──────────────────────────────────────────────
            modelBuilder.Entity<ShopFollower>(entity =>
            {
                // User relationship configured from IdentityAppUser block above.

                entity.HasOne(sf => sf.Shop)
                      .WithMany(s => s.ShopFollowers)
                      .HasForeignKey(sf => sf.ShopId);

                entity.HasIndex(sf => new { sf.ShopId, sf.UserId }).IsUnique();
            });
        }
    }
}
