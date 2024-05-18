using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.Service;
using Ecommerce.Service.src.ServiceAbstract;
using Moq;
using Xunit;

namespace Ecommerce.Tests
{
    public class ProductServiceTests
    {
        #region Fields 
        private readonly Mock<IProductRepo> _productRepoMock;
        private readonly Mock<ICategoryRepo> _categoryRepoMock;
        private readonly Mock<IProductImageRepo> _productImageRepoMock;
        private readonly Mock<IProductImageService> _productImageServiceMock;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;
        #endregion

        #region Constructors
        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepo>();
            _categoryRepoMock = new Mock<ICategoryRepo>();
            _productImageRepoMock = new Mock<IProductImageRepo>();
            _productImageServiceMock = new Mock<IProductImageService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductReadDto>();
                cfg.CreateMap<Category, CategoryReadDto>();
                cfg.CreateMap<ProductCreateDto, Product>();
                cfg.CreateMap<ProductUpdateDto, Product>();
                cfg.CreateMap<ProductImage, ProductImageReadDto>();
                cfg.CreateMap<ProductImageCreateDto, ProductImage>();
            });

            _mapper = config.CreateMapper();
            _productService = new ProductService(_productRepoMock.Object, _mapper, _categoryRepoMock.Object, _productImageRepoMock.Object, _productImageServiceMock.Object);
        }
        #endregion

        #region GET
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProductList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Description 1", Price = 100 },
                new Product { Id = Guid.NewGuid(), Title = "Product 2", Description = "Description 2", Price = 200 }
            };
            _productRepoMock.Setup(repo => repo.GetAllProductsAsync(null)).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(result);
        }

        [Fact]
        public async Task GetAllProductsAsync_WithPaginationOption_ShouldReturnProductList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Description 1", Price = 100 },
                new Product { Id = Guid.NewGuid(), Title = "Product 2", Description = "Description 2", Price = 200 }
            };
            var productQueryOptions = new ProductQueryOptions { Offset = 1, Limit = 10 };
            _productRepoMock.Setup(repo => repo.GetAllProductsAsync(productQueryOptions)).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync(productQueryOptions);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ShouldReturnProductList()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Title = "Product 1", Description = "Description 1", Price = 100, CategoryId = categoryId },
                new Product { Id = Guid.NewGuid(), Title = "Product 2", Description = "Description 2", Price = 200, CategoryId = categoryId }
            };
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(new Category { Id = categoryId });
            _productRepoMock.Setup(repo => repo.GetProductsByCategoryAsync(categoryId)).ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductsByCategoryAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.IsAssignableFrom<IEnumerable<ProductReadDto>>(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Title = "Product 1", Description = "Description 1", Price = 100, CategoryId = Guid.NewGuid() };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(product.CategoryId)).ReturnsAsync(new Category { Id = product.CategoryId });

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProductReadDto>(result);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidProductId_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNotFoundProduct_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _productService.GetProductByIdAsync(productId));
        }
        #endregion

        #region POST
        [Fact]
        public async Task CreateProductAsync_ShouldReturnCreatedProduct()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto { ProductTitle = "Product 1", ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };
            var product = new Product { Id = Guid.NewGuid(), Title = productCreateDto.ProductTitle, Description = productCreateDto.ProductDescription, Price = productCreateDto.ProductPrice, CategoryId = productCreateDto.CategoryId };

            _productRepoMock.Setup(repo => repo.CreateProductAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(product.CategoryId)).ReturnsAsync(new Category { Id = product.CategoryId });

            // Act
            var result = await _productService.CreateProductAsync(productCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProductReadDto>(result);
            Assert.Equal(product.CategoryId, result.CategoryId);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidCategory_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto { ProductTitle = "Product 1", ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync((Category)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Category not found", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithoutProductTitle_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto { ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Product title is required", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidImageData_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductTitle = "Product 1",
                ProductDescription = "Description 1",
                ProductPrice = 100,
                CategoryId = Guid.NewGuid(),
                ProductImages = new List<ProductImageCreateDto>
                { new ProductImageCreateDto { ImageData = null } }
            };

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync(new Category { Id = productCreateDto.CategoryId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Image Data cannot be empty or null", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithDuplicateProductTitle_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductTitle = "Existing Product",
                CategoryId = Guid.NewGuid(),
                ProductImages = new List<ProductImageCreateDto> { new ProductImageCreateDto { ImageData = "https://fastly.picsum.photos/id/491/200/200.jpg?hmac=Zi1sOp0NH_d3eOa3qUg8-oDQJWvIkH8UkrAJZ7l-4wg" } },
                ProductInventory = 10
            };

            var existingProduct = new Product
            {
                Id = Guid.NewGuid(),
                Title = "Existing Product",
                CategoryId = productCreateDto.CategoryId
            };

            _productRepoMock.Setup(repo => repo.GetAllProductsAsync(null)).ReturnsAsync(new List<Product> { existingProduct });
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync(new Category { Id = productCreateDto.CategoryId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Product title is already in use, please choose another title", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_NegativeProductInventory_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductTitle = "Valid Product",
                CategoryId = Guid.NewGuid(),
                ProductImages = new List<ProductImageCreateDto> { new ProductImageCreateDto { ImageData = "https://fastly.picsum.photos/id/491/200/200.jpg?hmac=Zi1sOp0NH_d3eOa3qUg8-oDQJWvIkH8UkrAJZ7l-4wg" } },
                ProductInventory = -1 // Negative inventory
            };

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync(new Category { Id = productCreateDto.CategoryId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Product inventory cannot be negative", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidProductPrice_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductTitle = "Product 1",
                ProductDescription = "Description 1",
                ProductPrice = -100,
                CategoryId = Guid.NewGuid()
            };

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync(new Category { Id = productCreateDto.CategoryId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Product price cannot be negative", exception.Message);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidProductTitle_ShouldThrowException()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductTitle = "",
                ProductDescription = "Description 1",
                ProductPrice = 100,
                CategoryId = Guid.NewGuid()
            };

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productCreateDto.CategoryId)).ReturnsAsync(new Category { Id = productCreateDto.CategoryId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.CreateProductAsync(productCreateDto));
            Assert.Equal("Product title must be at least 3 characters long", exception.Message);
        }
        #endregion

        #region PATCH
        [Fact]
        public async Task UpdateProductAsync_ShouldReturnUpdatedProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductTitle = "Product 1", ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };
            var product = new Product { Id = productId, Title = productUpdateDto.ProductTitle, Description = productUpdateDto.ProductDescription, Price = (decimal)productUpdateDto.ProductPrice, CategoryId = productUpdateDto.CategoryId.Value };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _productRepoMock.Setup(repo => repo.UpdateProductByIdAsync(It.IsAny<Product>())).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(product.CategoryId)).ReturnsAsync(new Category { Id = product.CategoryId });

            // Act
            var result = await _productService.UpdateProductByIdAsync(productId, productUpdateDto);
            result.ProductTitle = productUpdateDto.ProductTitle;
            result.ProductDescription = productUpdateDto.ProductDescription;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ProductReadDto>(result);
            Assert.Equal(product.CategoryId, result.CategoryId);
            Assert.Equal(product.Title, result.ProductTitle);
            Assert.Equal(product.Description, result.ProductDescription);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidProductId_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.Empty;
            var productUpdateDto = new ProductUpdateDto { ProductTitle = "Product 1", ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("ProductId is required", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNotFoundProduct_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductTitle = "Product 1", ProductDescription = "Description 1", ProductPrice = 100, CategoryId = Guid.NewGuid() };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("Product not found", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidCategory_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto
            {
                ProductTitle = "Product 1",
                ProductDescription = "Description 1",
                ProductPrice = 100,
                CategoryId = Guid.NewGuid()
            };
            var product = new Product
            {
                Id = productId,
                Title = productUpdateDto.ProductTitle,
                Description = productUpdateDto.ProductDescription,
                Price = (decimal)productUpdateDto.ProductPrice,
                CategoryId = productUpdateDto.CategoryId.Value
            };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);

            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value)).ReturnsAsync((Category)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("Category not found", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidProductPrice_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto
            {
                ProductTitle = "Product 1",
                ProductDescription = "Description 1",
                ProductPrice = -100,
                CategoryId = Guid.NewGuid()
            };
            var product = new Product
            {
                Id = productId,
                Title = productUpdateDto.ProductTitle,
                Description = productUpdateDto.ProductDescription,
                Price = (decimal)productUpdateDto.ProductPrice,
                CategoryId = productUpdateDto.CategoryId.Value
            };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value)).ReturnsAsync(new Category { Id = productUpdateDto.CategoryId.Value });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("Product price cannot be negative", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidProductTitle_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto
            {
                ProductTitle = "",
                ProductDescription = "Description 1",
                ProductPrice = 100,
                CategoryId = Guid.NewGuid()
            };
            var product = new Product
            {
                Id = productId,
                Title = "Original Title",
                Description = "Original Description",
                Price = 50,
                CategoryId = productUpdateDto.CategoryId.Value
            };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value)).ReturnsAsync(new Category { Id = productUpdateDto.CategoryId.Value });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("Product title must be at least 3 characters long", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidProductInventory_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto
            {
                ProductTitle = "Product 1",
                ProductDescription = "Description 1",
                ProductPrice = 100,
                CategoryId = Guid.NewGuid(),
                ProductInventory = -1
            };
            var product = new Product
            {
                Id = productId,
                Title = productUpdateDto.ProductTitle,
                Description = productUpdateDto.ProductDescription,
                Price = (decimal)productUpdateDto.ProductPrice,
                CategoryId = productUpdateDto.CategoryId.Value
            };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value)).ReturnsAsync(new Category { Id = productUpdateDto.CategoryId.Value });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _productService.UpdateProductByIdAsync(productId, productUpdateDto));
            Assert.Equal("Product inventory cannot be negative", exception.Message);
        }
        #endregion

        #region DELETE
        [Fact]
        public async Task DeleteProductAsync_ShouldReturnTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Title = "Product 1", Description = "Description 1", Price = 100, CategoryId = Guid.NewGuid() };

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _productRepoMock.Setup(repo => repo.DeleteProductByIdAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProductByIdAsync(productId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_WithNotFoundProduct_ShouldReturnFalse()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _productRepoMock.Setup(repo => repo.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.DeleteProductByIdAsync(productId);

            // Assert
            Assert.False(result);
        }
        #endregion
    }
}
