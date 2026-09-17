using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Data;
using HandMade.Infrastructure.Identity.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HandMade.Infrastructure.Helpers
{
    /// <summary>
    /// Development-only seeder that fills the database with a small but full-spectrum
    /// set of users, shops, categories, products and images for manual endpoint testing.
    /// Bypasses the CQRS/business layer on purpose so every entity state is reachable
    /// (e.g. published products, suspended shops) — states the public API cannot produce.
    /// Wired from Program.cs behind <c>IsDevelopment()</c> + the <c>SeedTestData</c> flag.
    /// Idempotent: a sentinel account short-circuits re-runs.
    /// </summary>
    public static class TestDataSeeder
    {
        private const string Password = "Test@123";
        private const string SentinelEmail = "artist1@handmade.test";

        public static async Task SeedTestDataAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityAppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityAppRole>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            if (await userManager.FindByEmailAsync(SentinelEmail) is not null)
            {
                Console.WriteLine("TestDataSeeder: sentinel account found — skipping.");
                return;
            }

            Console.WriteLine("TestDataSeeder: seeding development test data...");

            await EnsureRolesAsync(roleManager);

            // ── Users ──────────────────────────────────────────────
            var admin = await CreateUserAsync(userManager, "admin1", "admin1@handmade.test",
                isSeller: false, roles: new[] { nameof(AssignedRole.Admin) });

            var artists = new List<IdentityAppUser>();
            for (int i = 1; i <= 5; i++)
            {
                artists.Add(await CreateUserAsync(userManager, $"artist{i}", $"artist{i}@handmade.test",
                    isSeller: true, roles: new[] { nameof(AssignedRole.Artist), nameof(AssignedRole.Client) }));
            }

            var clients = new List<IdentityAppUser>();
            for (int i = 1; i <= 4; i++)
            {
                clients.Add(await CreateUserAsync(userManager, $"client{i}", $"client{i}@handmade.test",
                    isSeller: false, roles: new[] { nameof(AssignedRole.Client) }));
            }

            var now = DateTime.UtcNow;

            // ── Addresses (one default per user) ───────────────────
            var addressesByUser = new Dictionary<Guid, Address>();
            foreach (var user in new[] { admin }.Concat(artists).Concat(clients))
            {
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Label = "Home",
                    DetailedAddress = $"{user.UserName} street 10, Test City",
                    IsDefault = true,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                db.Addresses.Add(address);
                addressesByUser[user.Id] = address;
            }

            // ── Categories + SubCategories ─────────────────────────
            var categorySpecs = new[]
            {
                ("Jewelry",   "Handmade rings, necklaces and bracelets", new[] { "Rings", "Necklaces", "Bracelets" }),
                ("Ceramics",  "Hand-thrown pottery and tableware",       new[] { "Mugs", "Bowls", "Vases" }),
                ("Textiles",  "Woven, knitted and embroidered pieces",   new[] { "Scarves", "Blankets" }),
                ("Woodwork",  "Carved and turned wooden objects",        new[] { "Boards", "Utensils" }),
            };

            var categories = new List<Category>();
            var subCategories = new List<SubCategory>();
            int catIndex = 0;
            foreach (var (name, description, subNames) in categorySpecs)
            {
                catIndex++;
                var category = new Category
                {
                    Id = Det(1000 + catIndex),
                    Name = name,
                    Description = description,
                    ImageUrl = $"uploads/general/seed-cat-{catIndex}.jpg",
                    CreatedAt = now,
                    UpdatedAt = now
                };
                categories.Add(category);
                db.Categories.Add(category);

                int subIndex = 0;
                foreach (var subName in subNames)
                {
                    subIndex++;
                    var sub = new SubCategory
                    {
                        Id = Det(2000 + catIndex * 10 + subIndex),
                        CategoryId = category.Id,
                        Name = subName,
                        CreatedAt = now,
                        UpdatedAt = now
                    };
                    subCategories.Add(sub);
                    db.SubCategories.Add(sub);
                }
            }

            // ── Shops (one per artist, spread across every ShopStatus) ──
            var shopStatuses = new[]
            {
                ShopStatus.Active, ShopStatus.Active, ShopStatus.Pending,
                ShopStatus.Rejected, ShopStatus.Suspended
            };

            var shops = new List<Shop>();
            for (int i = 0; i < artists.Count; i++)
            {
                var status = shopStatuses[i];
                var shop = new Shop
                {
                    Id = Det(3000 + i + 1),
                    OwnerUserId = artists[i].Id,
                    Name = $"{artists[i].UserName}'s Workshop",
                    Description = $"Handmade goods crafted by {artists[i].UserName}.",
                    ImageUrl = $"uploads/shops/seed-shop-{i + 1}.jpg",
                    Status = status,
                    RejectionMessage = status == ShopStatus.Rejected
                        ? "Shop imagery does not meet the storefront guidelines."
                        : null,
                    RatingAverage = status == ShopStatus.Active ? 4.20m + i * 0.15m : 0m,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                shops.Add(shop);
                db.Shops.Add(shop);
            }

            // ── Products ───────────────────────────────────────────
            // Full matrix: ApprovalStatus {Pending, Approved, Rejected}
            //            × ProductStatus  {Active, InActive}
            //            × IsPublished    {true, false}
            // Approved shops (index 0,1) carry the matrix; the Pending / Rejected /
            // Suspended shops (2,3,4) get a couple of products each for admin coverage.
            var products = new List<Product>();
            var productImages = new List<ProductImage>();
            int productCounter = 0;

            void AddProduct(Shop shop, int catOrdinal, ProductApprovalStatus approval,
                ProductStatus status, bool published, decimal price)
            {
                productCounter++;
                var category = categories[catOrdinal % categories.Count];
                var sub = subCategories.First(s => s.CategoryId == category.Id);

                var product = new Product
                {
                    Id = Det(4000 + productCounter),
                    ShopId = shop.Id,
                    CategoryId = category.Id,
                    SubCategoryId = sub.Id,
                    Title = $"{category.Name} piece #{productCounter}",
                    Description = $"A one-of-a-kind {category.Name.ToLower()} item, {approval}/{status}.",
                    SKU = $"SEED-{shop.Id.ToString()[^4..]}-{productCounter:D3}",
                    Price = price,
                    Status = status,
                    ApprovalStatus = approval,
                    IsPublished = published,
                    RejectionMessage = approval == ProductApprovalStatus.Rejected
                        ? "Photos are too low-resolution for the storefront."
                        : null,
                    ExpectedDays = 3 + (productCounter % 12),
                    CreatedAt = now.AddMinutes(-productCounter),
                    UpdatedAt = now
                };
                products.Add(product);
                db.Products.Add(product);

                int imageCount = 1 + (productCounter % 3);
                for (int k = 0; k < imageCount; k++)
                {
                    productImages.Add(new ProductImage
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Url = $"uploads/products/seed-{productCounter}-{k}.jpg",
                        AltText = $"{product.Title} — view {k + 1}",
                        SortOrder = k,
                        IsPrimary = k == 0,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            var approvals = new[]
            {
                ProductApprovalStatus.Pending,
                ProductApprovalStatus.Approved,
                ProductApprovalStatus.Rejected
            };
            var statuses = new[] { ProductStatus.Active, ProductStatus.InActive };
            var publishedFlags = new[] { true, false };

            int matrixIndex = 0;
            foreach (var approval in approvals)
                foreach (var status in statuses)
                    foreach (var published in publishedFlags)
                    {
                        var shop = shops[matrixIndex % 2];           // the two Active shops
                        AddProduct(shop, matrixIndex, approval, status, published,
                            price: 25m + matrixIndex * 7.5m);
                        matrixIndex++;
                    }

            // A few products under the non-approved shops (admin dashboard coverage).
            AddProduct(shops[2], 0, ProductApprovalStatus.Pending, ProductStatus.InActive, false, 40m);
            AddProduct(shops[2], 1, ProductApprovalStatus.Pending, ProductStatus.InActive, false, 55m);
            AddProduct(shops[3], 2, ProductApprovalStatus.Rejected, ProductStatus.InActive, false, 30m);
            AddProduct(shops[4], 3, ProductApprovalStatus.Approved, ProductStatus.InActive, false, 65m);

            // Extra storefront-visible catalogue (Approved + Published + Active) across
            // both active shops and every category, so the public endpoints return real pages.
            for (int c = 0; c < categories.Count; c++)
            {
                AddProduct(shops[0], c, ProductApprovalStatus.Approved, ProductStatus.Active, true, 35m + c * 10m);
                AddProduct(shops[1], c, ProductApprovalStatus.Approved, ProductStatus.Active, true, 45m + c * 10m);
            }

            db.ProductImages.AddRange(productImages);

            // ── Carts, Orders, Payments ────────────────────────────
            // One order per OrderStatus so the client journey is walkable end to end
            // before the artist-side endpoints exist (quoting and marking complete are
            // the artist story). Also a live cart to drive the checkout fan-out.
            var visibleProducts = products
                .Where(p => p.ApprovalStatus == ProductApprovalStatus.Approved
                            && p.Status == ProductStatus.Active
                            && p.IsPublished
                            && p.ShopId == shops[0].Id)
                .ToList();

            var orders = new List<Order>();
            var payments = new List<Payment>();
            int orderCounter = 0;

            Order AddOrder(IdentityAppUser buyer, Product product, OrderStatus status)
            {
                orderCounter++;
                var shippingAddress = addressesByUser[buyer.Id];
                var quantity = 1 + (orderCounter % 3);
                var subtotal = product.Price * quantity;

                var order = new Order
                {
                    Id = Det(5000 + orderCounter),
                    UserId = buyer.Id,
                    ShopId = product.ShopId,
                    ProductId = product.Id,
                    ShippingAddressId = shippingAddress.Id,
                    OrderNumber = $"HM-SEED-{orderCounter:D4}",
                    Status = status,
                    Quantity = quantity,
                    UnitPriceSnapshot = product.Price,
                    ProductTitleSnapshot = product.Title,
                    ProductImageSnapshot = $"uploads/products/seed-order-{orderCounter}.jpg",
                    Subtotal = subtotal,
                    ShippingFee = 15m,
                    TaxTotal = 0m,
                    GrandTotal = subtotal + 15m,
                    SpecialInstructions = $"Seeded order in {status} state.",
                    CreatedAt = now.AddDays(-orderCounter),
                    UpdatedAt = now
                };

                // The artist quote lands from BuyerPending onwards.
                if (status is not OrderStatus.SellerPending)
                    order.ExecutionDays = 5 + (orderCounter % 10);

                // Money only moves once the buyer has accepted.
                if (status is OrderStatus.InProgress or OrderStatus.CompletedBySeller
                    or OrderStatus.Delivered or OrderStatus.Refunded)
                    order.ConfirmedAt = now.AddDays(-orderCounter).AddHours(2);

                if (status is OrderStatus.CompletedBySeller)
                    order.AutoReleaseAt = now.AddDays(7);

                if (status is OrderStatus.Cancelled)
                {
                    order.CancellationReason = "Changed my mind about the design.";
                    order.CancelledByUserId = buyer.Id;
                    order.CancelledAt = now.AddDays(-orderCounter).AddHours(3);
                }

                orders.Add(order);
                db.Orders.Add(order);

                db.OrderAttachments.Add(new OrderAttachment
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    Url = $"uploads/orders/seed-reference-{orderCounter}.jpg",
                    SortOrder = 0,
                    CreatedAt = now,
                    UpdatedAt = now
                });

                if (order.ConfirmedAt is not null)
                {
                    var escrow = status switch
                    {
                        OrderStatus.Delivered => EscrowStatus.Released,
                        OrderStatus.Refunded => EscrowStatus.Refunded,
                        _ => EscrowStatus.Held
                    };

                    var payment = new Payment
                    {
                        Id = Det(6000 + orderCounter),
                        OrderId = order.Id,
                        UserId = buyer.Id,
                        Amount = order.GrandTotal,
                        Currency = "EGP",
                        Method = PaymentMethod.CreditCard,
                        Status = status == OrderStatus.Refunded
                            ? PaymentStatus.Refunded
                            : PaymentStatus.Completed,
                        EscrowStatus = escrow,
                        ProviderRef = $"FAKE-CH-SEED{orderCounter:D4}",
                        IdempotencyKey = $"order-{order.Id}",
                        PaidAt = order.ConfirmedAt,
                        EscrowReleasedAt = escrow == EscrowStatus.Released ? now.AddDays(-1) : null,
                        CreatedAt = order.ConfirmedAt.Value,
                        UpdatedAt = now
                    };

                    payments.Add(payment);
                    db.Payments.Add(payment);

                    if (status == OrderStatus.Refunded)
                    {
                        db.Refunds.Add(new Refund
                        {
                            Id = Det(6500 + orderCounter),
                            OrderId = order.Id,
                            PaymentId = payment.Id,
                            Amount = payment.Amount,
                            Status = RefundStatus.Completed,
                            Reason = "Seeded refund.",
                            CreatedAt = now,
                            UpdatedAt = now
                        });
                    }
                }

                return order;
            }

            var allOrderStatuses = new[]
            {
                OrderStatus.SellerPending,
                OrderStatus.BuyerPending,
                OrderStatus.InProgress,
                OrderStatus.CompletedBySeller,
                OrderStatus.Delivered,
                OrderStatus.Cancelled,
                OrderStatus.Refunded
            };

            for (int i = 0; i < allOrderStatuses.Length; i++)
                AddOrder(clients[0], visibleProducts[i % visibleProducts.Count], allOrderStatuses[i]);

            // A second Delivered order on a different product, so review eligibility can
            // be exercised twice and the duplicate-review 409 checked on the first.
            AddOrder(clients[0], visibleProducts[(allOrderStatuses.Length + 1) % visibleProducts.Count],
                OrderStatus.Delivered);

            var cart = new Cart
            {
                Id = Det(7001),
                UserId = clients[1].Id,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Carts.Add(cart);

            int cartItemCounter = 0;
            foreach (var cartProduct in visibleProducts.Take(2))
            {
                cartItemCounter++;
                db.CartItems.Add(new CartItem
                {
                    Id = Det(7100 + cartItemCounter),
                    CartId = cart.Id,
                    UserId = clients[1].Id,
                    ProductId = cartProduct.Id,
                    Quantity = cartItemCounter,
                    ProductName = cartProduct.Title,
                    UnitPrice = cartProduct.Price,
                    TotalPrice = cartProduct.Price * cartItemCounter,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }


            await db.SaveChangesAsync();

            Console.WriteLine(
                $"TestDataSeeder: done. users={1 + artists.Count + clients.Count}, " +
                $"shops={shops.Count}, categories={categories.Count}, " +
                $"products={products.Count}, images={productImages.Count}, " +
                $"orders={orders.Count}, payments={payments.Count}. " +
                $"Password for every seeded account: {Password}");
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityAppRole> roleManager)
        {
            foreach (var role in new[]
            {
                nameof(AssignedRole.Admin), nameof(AssignedRole.Artist), nameof(AssignedRole.Client)
            })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityAppRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpperInvariant(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Description = $"{role} role (seeded by TestDataSeeder)",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        private static async Task<IdentityAppUser> CreateUserAsync(
            UserManager<IdentityAppUser> userManager,
            string userName, string email, bool isSeller, string[] roles)
        {
            var now = DateTime.UtcNow;
            var user = new IdentityAppUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = "+100000000000",
                IsSeller = isSeller,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            var result = await userManager.CreateAsync(user, Password);
            if (!result.Succeeded)
                throw new Exception(
                    $"TestDataSeeder: failed to create '{userName}': " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRolesAsync(user, roles);
            return user;
        }

        /// <summary>Deterministic GUID (00000000-0000-0000-0000-00000000NNNN) for stable, paste-able seed IDs.</summary>
        private static Guid Det(int n) => new($"00000000-0000-0000-0000-{n:D12}");
    }
}
