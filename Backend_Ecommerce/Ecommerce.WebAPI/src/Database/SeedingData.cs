using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;
using Ecommerce.Service.src.ServiceAbstract;
using Ecommerce.WebAPI.src.ExternalService;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Ecommerce.WebAPI.src.Database
{
    public class SeedingData
    {
        private static Random random = new Random();
        private static int GetRandomNumber()
        {
            return random.Next(1, 11);
        }
        private static int GetRandomNumberForImage()
        {
            return random.Next(100, 1000);
        }

        private static int RandomNumber1 = GetRandomNumber();
        private static int RandomNumber2 = GetRandomNumber();
        private static int RandomNumber3 = GetRandomNumber();
        private static int RandomNumber4 = GetRandomNumber();

        #region Categories
        private static Category category1 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Men",
            Image = $"https://picsum.photos/200/?random={RandomNumber1}"
        };
        private static Category category2 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Women",
            Image = $"https://picsum.photos/200/?random={RandomNumber2}"
        };
        private static Category category3 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            Image = $"https://picsum.photos/200/?random={RandomNumber3}"
        };
        private static Category category4 = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Jewelery",
            Image = $"https://picsum.photos/200/?random={RandomNumber4}"
        };

        public static List<Category> GetCategories()
        {
            return new List<Category>
            {
                category1, category2, category3, category4
            };
        }
        #endregion

        #region Products
        private static List<Product> GenerateProductsForCategory(Category category, int count)
        {
            var products = new List<Product>();

            for (int i = 1; i <= count; i++)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Title = $"{category.Name} Product {i}",
                    Description = $"Description for {category.Name} Product {i}",
                    Price = GetRandomNumber() * 100,
                    CategoryId = category.Id,
                    Inventory = 100
                };
                products.Add(product);
            }
            return products;
        }

        public static List<Product> GetProducts()
        {
            var products = new List<Product>();
            products.AddRange(GenerateProductsForCategory(category1, 10));
            products.AddRange(GenerateProductsForCategory(category2, 10));
            products.AddRange(GenerateProductsForCategory(category3, 10));
            products.AddRange(GenerateProductsForCategory(category4, 10));
            return products;
        }

        private static List<Product> Products = GetProducts();

        public static List<ProductImage> GetProductImagesForProduct(Guid productId)
        {
            var productImages = new List<ProductImage>();
            for (int i = 0; i < 3; i++)
            {
                var productImage = new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageData = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
                    ProductId = productId
                };
                productImages.Add(productImage);
            }
            return productImages;
        }
        #endregion

        #region Users
        public static List<User> GetUsers()
        {
            var passwordService = new PasswordService();
            var hashedAdminPassword = passwordService.HashPassword("admin@123", out byte[] adminSalt);
            var hashedBinhPassword = passwordService.HashPassword("binh@123", out byte[] binhSalt);
            var hashedAdnanPassword = passwordService.HashPassword("adnan@123", out byte[] adnanSalt);
            var hashedYuankePassword = passwordService.HashPassword("yuanke@123", out byte[] yuankeSalt);
            var hashedCustomerPassword = passwordService.HashPassword("customer@123", out byte[] customerSalt);
            return new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin1",
                    Email = "john@example.com",
                    Password = hashedAdminPassword,
                    Salt= adminSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    UserRole = UserRole.Admin,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Binh",
                    Email = "binh@admin.com",
                    Password = hashedBinhPassword,
                    Salt= binhSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    UserRole = UserRole.Admin,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Adnan",
                    Email = "adnan@admin.com",
                    Password = hashedAdnanPassword,
                    Salt= adnanSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    UserRole = UserRole.Admin,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Yuanke",
                    Email = "yuanke@admin.com",
                    Password = hashedYuankePassword,
                    Salt= yuankeSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    UserRole = UserRole.Admin,
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Customer1",
                    Email = "customer1@customer.com",
                    Password = hashedCustomerPassword,
                    Salt= customerSalt,
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage}",
                    UserRole = UserRole.Customer,
                }
            };
        }
        #endregion
    }
}