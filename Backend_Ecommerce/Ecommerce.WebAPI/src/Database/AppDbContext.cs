using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Database
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        #region Properties
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<ShippingInfo> ShippingInfos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        #endregion

        #region Constructors
        static AppDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public AppDbContext(DbContextOptions options, IConfiguration config) : base(options)
        {
            _configuration = config;
        }
        #endregion

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConvertIdPropertiesToUUID(modelBuilder);
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OrderStatus>();

            base.OnModelCreating(modelBuilder);

            // Enum columns
            modelBuilder.Entity<User>(entity => entity.Property(u => u.UserRole).HasColumnType("user_role"));
            modelBuilder.Entity<Order>(entity => entity.Property(o => o.Status).HasColumnType("order_status"));

            // Relationship
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderProducts)
                .WithOne()
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingInfo)
                .WithOne(si => si.Order)
                .HasForeignKey<ShippingInfo>(si => si.OrderId);

            modelBuilder.Entity<CartItem>()
                .HasKey(op => new { op.CartId, op.ProductId });

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            // constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Automatically set CreatedDate when creating data
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductImage>()
                .Property(i => i.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Category>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ProductImage>()
                .Property(i => i.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ShippingInfo>()
                .Property(si => si.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Cart>()
                .Property(c => c.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Review>()
                .Property(r => r.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Setting column type
            modelBuilder.Entity<User>(u => u.Property(u => u.Name).HasColumnType("varchar(255)"));
            modelBuilder.Entity<User>(u => u.Property(u => u.Password).HasColumnType("varchar"));
            modelBuilder.Entity<User>(u => u.Property(u => u.Email).HasColumnType("varchar(255)"));
            modelBuilder.Entity<User>(u => u.Property(u => u.Avatar).HasColumnType("varchar(255)"));
            modelBuilder.Entity<User>(u => u.Property(u => u.Salt).HasColumnType("bytea"));

            modelBuilder.Entity<Category>(c => c.Property(c => c.Name).HasColumnType("varchar(20)"));
            modelBuilder.Entity<Category>(c => c.Property(c => c.Image).HasColumnType("varchar"));

            modelBuilder.Entity<ProductImage>(i => i.Property(i => i.ImageData).HasColumnType("varchar"));

            // Relationship, column type and constraint of Product
            modelBuilder.Entity<Product>(product =>
            {
                // Configure the foreign key relationship
                product.HasOne(p => p.Category)
                    .WithMany(c => c.Products) // Assuming Category has a Products collection
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
                product.HasMany(p => p.ProductImages)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure column type and constraint of Product
                product.Property(p => p.Title).IsRequired().HasColumnType("varchar").HasMaxLength(255);
                product.HasIndex(p => p.Title).IsUnique().HasDatabaseName("title");
                product.Property(p => p.Description).IsRequired().HasColumnType("varchar");
                product.HasIndex(p => p.Price);
                product.Property(p => p.Price).IsRequired();
                product.Property(p => p.Inventory).HasDefaultValue(0);
                product.ToTable(t => t.HasCheckConstraint("product_price_check", "price > 0"));
                product.ToTable(t => t.HasCheckConstraint("product_inventory_check", "inventory >= 0"));
            });

            // Fetch seed data
            SeedData(modelBuilder);
        }

        #endregion

        #region Helper Methods
        private void ConvertIdPropertiesToUUID(ModelBuilder modelBuilder)
        {
            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var idProperty = entityType.FindProperty("Id");
                if (idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.SetColumnType("uuid");
                }
            }
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            var categories = SeedingData.GetCategories();
            modelBuilder.Entity<Category>().HasData(categories);

            var products = SeedingData.GetProducts();
            modelBuilder.Entity<Product>().HasData(products);

            var productImages = new List<ProductImage>();
            foreach (var product in products)
            {
                var imagesForProduct = SeedingData.GetProductImagesForProduct(product.Id);
                productImages.AddRange(imagesForProduct);
            }
            modelBuilder.Entity<ProductImage>().HasData(productImages);

            var users = SeedingData.GetUsers();
            modelBuilder.Entity<User>().HasData(users);

        }
        #endregion

        #region Queries function
        #endregion
    }
}