using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.Service;
using Moq;
using Xunit;

namespace Ecommerce.Test.src.Service
{
    public class CategoryServiceTest
    {
        private readonly CategoryService _categoryService;
        private readonly Mock<ICategoryRepo> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;

        public CategoryServiceTest()
        {
            _categoryRepoMock = new Mock<ICategoryRepo>();
            _mapperMock = new Mock<IMapper>();
            _categoryService = new CategoryService(_mapperMock.Object, _categoryRepoMock.Object);
        }

        private Category CreateCategory(Guid id, string name, string image)
        {
            return new Category { Id = id, Name = name, Image = image };
        }

        private CategoryReadDto CreateCategoryReadDto(Guid id, string name, string image)
        {
            return new CategoryReadDto { CategoryId = id, CategoryName = name, CategoryImage = image };
        }

        private void SetupCategoryMapping(Category category, CategoryReadDto categoryReadDto)
        {
            _mapperMock.Setup(mapper => mapper.Map<Category, CategoryReadDto>(category))
                       .Returns(categoryReadDto);
        }

        private void SetupCategoryRepoGetAll(IEnumerable<Category> categories)
        {
            _categoryRepoMock.Setup(repo => repo.GetAllCategoriesAsync())
                             .ReturnsAsync(categories);
        }

        private void SetupCategoryRepoGetById(Guid categoryId, Category category)
        {
            _categoryRepoMock.Setup(repo => repo.GetCategoryByIdAsync(categoryId))
                             .ReturnsAsync(category);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsCategoryReadDtos()
        {
            // Arrange
            var mockCategories = new List<Category>
            {
                CreateCategory(Guid.NewGuid(), "Category 1", "image1.jpg"),
                CreateCategory(Guid.NewGuid(), "Category 2", "image2.jpg")
            };

            var categoryReadDtos = mockCategories.Select(c => CreateCategoryReadDto(c.Id, c.Name, c.Image)).ToList();

            SetupCategoryRepoGetAll(mockCategories);

            foreach (var category in mockCategories)
            {
                var categoryReadDto = categoryReadDtos.First(dto => dto.CategoryId == category.Id);
                SetupCategoryMapping(category, categoryReadDto);
            }

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.Equal(categoryReadDtos, result);
            
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ValidId_ReturnsCategoryReadDto()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = CreateCategory(categoryId, "Category 1", "image1.jpg");
            var categoryReadDto = CreateCategoryReadDto(categoryId, "Category 1", "image1.jpg");

            SetupCategoryRepoGetById(categoryId, category);
            SetupCategoryMapping(category, categoryReadDto);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.Equal(categoryReadDto, result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var categoryId = Guid.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _categoryService.GetCategoryByIdAsync(categoryId));
            Assert.Equal("bad request", exception.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidInput_ReturnsCategoryReadDto()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreateDto { CategoryName = "New Category", CategoryImage = "image.jpg" };
            var category = CreateCategory(Guid.NewGuid(), "New Category", "image.jpg");
            var categoryReadDto = CreateCategoryReadDto(category.Id, "New Category", "image.jpg");

            _mapperMock.Setup(mapper => mapper.Map<CategoryCreateDto, Category>(categoryCreateDto))
                       .Returns(category);

            _categoryRepoMock.Setup(repo => repo.CreateCategoryAsync(category))
                             .ReturnsAsync(category);

            _mapperMock.Setup(mapper => mapper.Map<Category, CategoryReadDto>(category))
                       .Returns(categoryReadDto);

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryCreateDto);

            // Assert
            Assert.Equal(categoryReadDto, result);
        }

        [Theory]
        [InlineData("", "Category name cannot be empty")]
        [InlineData("ThisNameIsWayTooLongForTheValidationCheck", "Category name cannot be longer than 20 characters")]
        public async Task CreateCategoryAsync_InvalidName_ThrowsAppException(string name, string expectedMessage)
        {
            // Arrange
            var categoryCreateDto = new CategoryCreateDto { CategoryName = name, CategoryImage = "image.jpg" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _categoryService.CreateCategoryAsync(categoryCreateDto));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public async Task CreateCategoryAsync_InvalidImageFormat_ThrowsAppException()
        {
            // Arrange
            var categoryCreateDto = new CategoryCreateDto { CategoryName = "Valid Name", CategoryImage = "image.pdf" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _categoryService.CreateCategoryAsync(categoryCreateDto));
            Assert.Equal("Category image can only be jpg|jpeg|png|gif|bmp", exception.Message);
        }

        [Fact]
        public async Task UpdateCategoryByIdAsync_ValidInput_ReturnsUpdatedCategoryReadDto()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryUpdateDto = new CategoryUpdateDto { CategoryName = "Updated Category", CategoryImage = "updated_image.jpg" };
            var category = CreateCategory(categoryId, "Original Name", "original_image.jpg");
            var updatedCategory = CreateCategory(categoryId, "Updated Category", "updated_image.jpg");
            var categoryReadDto = CreateCategoryReadDto(categoryId, "Updated Category", "updated_image.jpg");

            SetupCategoryRepoGetById(categoryId, category);

            _categoryRepoMock.Setup(repo => repo.UpdateCategoryByIdAsync(category))
                             .ReturnsAsync(updatedCategory);

            _mapperMock.Setup(mapper => mapper.Map<Category, CategoryReadDto>(updatedCategory))
                       .Returns(categoryReadDto);

            // Act
            var result = await _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto);

            // Assert
            Assert.Equal(categoryReadDto, result);
        }

        [Theory]
        [InlineData("", "Category name cannot be empty")]
        [InlineData("ThisNameIsWayTooLongForTheValidationCheck", "Category name cannot be longer than 20 characters")]
        public async Task UpdateCategoryByIdAsync_InvalidName_ThrowsAppException(string name, string expectedMessage)
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryUpdateDto = new CategoryUpdateDto { CategoryName = name };
            var category = CreateCategory(categoryId, "Original Name", "original_image.jpg");

            SetupCategoryRepoGetById(categoryId, category);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public async Task UpdateCategoryByIdAsync_InvalidImageFormat_ThrowsAppException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryUpdateDto = new CategoryUpdateDto { CategoryImage = "invalid_image.pdf" };
            var category = CreateCategory(categoryId, "Original Name", "original_image.jpg");

            SetupCategoryRepoGetById(categoryId, category);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto));
            Assert.Equal("Category image can only be jpg|jpeg|png|gif|bmp", exception.Message);
        }

        [Fact]
        public async Task DeleteCategoryByIdAsync_ValidId_ReturnsTrue()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _categoryRepoMock.Setup(repo => repo.DeleteCategoryByIdAsync(categoryId))
                             .ReturnsAsync(true);

            // Act
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCategoryByIdAsync_NotFound_ThrowsAppException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _categoryRepoMock.Setup(repo => repo.DeleteCategoryByIdAsync(categoryId))
                             .ThrowsAsync(AppException.NotFound("Category not found"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _categoryService.DeleteCategoryByIdAsync(categoryId));
            Assert.Equal("Category not found", exception.Message);
        }
    }
}
