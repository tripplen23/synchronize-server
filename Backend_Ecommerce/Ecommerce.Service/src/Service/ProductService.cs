using System.Text.RegularExpressions;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class ProductService : IProductService
    {
        #region Fields
        private readonly IProductRepo _productRepo;
        private IMapper _mapper;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductImageService _productImageService;
        #endregion

        #region Constructors
        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo, IProductImageRepo productImageRepo, IProductImageService productImageService)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _productImageService = productImageService;
        }
        #endregion

        #region GET
        public async Task<IEnumerable<ProductReadDto>> GetAllProductsAsync(ProductQueryOptions? productQueryOptions)
        {
            try
            {
                var products = await _productRepo.GetAllProductsAsync(productQueryOptions);
                var productDtos = _mapper.Map<List<ProductReadDto>>(products);

                foreach (var productDto in productDtos)
                {
                    // Fetch category information for the product
                    var category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);
                    var categoryDto = _mapper.Map<CategoryReadDto>(category);

                    // Set the category property in the product DTO
                    productDto.Category = categoryDto;
                }

                return productDtos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ProductReadDto>> GetProductsByCategoryAsync(Guid categoryId)
        {
            var foundCategory = await _categoryRepo.GetCategoryByIdAsync(categoryId);
            if (foundCategory is null)
            {
                throw AppException.NotFound("Category not found");
            }
            var products = await _productRepo.GetProductsByCategoryAsync(categoryId);
            var productDtos = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductReadDto>>(products);

            return productDtos;
        }

        public async Task<ProductReadDto> GetProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw AppException.BadRequest("ProductId is required");
            }
            try
            {
                var foundproduct = await _productRepo.GetProductByIdAsync(productId);
                if (foundproduct is null)
                {
                    throw AppException.NotFound("Product not found");
                }
                var productDto = _mapper.Map<ProductReadDto>(foundproduct);

                // Fetch category information for the product
                var category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);

                // Set the category property in the product DTO
                productDto.Category = categoryDto;
                return productDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region POST
        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto newProduct)
        {
            try
            {
                var productList = await _productRepo.GetAllProductsAsync(null);
                if (newProduct is null)
                {
                    throw AppException.BadRequest("Product cannot be null");
                }

                // Validate image data
                if (newProduct.ProductImages != null)
                {
                    foreach (var image in newProduct.ProductImages)
                    {
                        // Check if the image data is provided
                        if (string.IsNullOrWhiteSpace(image.ImageData))
                        {
                            throw AppException.InvalidInputException("Image Data cannot be empty or null");
                        }
                        // Check if the ImageData points to a valid image format 
                        if (!IsImageDataValid(image.ImageData))
                        {
                            throw AppException.InvalidInputException("Invalid image format");
                        }
                    }
                }

                if (productList.Any(p => p.Title == newProduct.ProductTitle))
                {
                    throw AppException.DuplicateProductTitleException("Product title is already in use, please choose another title");
                }

                if (newProduct.ProductInventory < 0)
                {
                    throw AppException.InvalidInputException("Product inventory cannot be negative");
                }
                if (newProduct.ProductPrice < 0)
                {
                    throw AppException.InvalidInputException("Product price cannot be negative");
                }
                if (newProduct.ProductTitle is null)
                {
                    throw AppException.InvalidInputException("Product title is required");
                }
                if (newProduct.ProductTitle.Length < 3)
                {
                    throw AppException.InvalidInputException("Product title must be at least 3 characters long");
                }

                // Check if the specified category ID exists
                var category = await _categoryRepo.GetCategoryByIdAsync(newProduct.CategoryId);
                if (category == null)
                {
                    throw AppException.NotFound("Category not found");
                }

                var productEntity = _mapper.Map<Product>(newProduct);
                productEntity.ProductImages = _mapper.Map<List<ProductImage>>(newProduct.ProductImages);
                productEntity.Inventory = _mapper.Map<int>(newProduct.ProductInventory);

                var createdProduct = await _productRepo.CreateProductAsync(productEntity);

                var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);
                productReadDto.Category = _mapper.Map<CategoryReadDto>(category);

                return productReadDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region DELETE
        public async Task<bool> DeleteProductByIdAsync(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                throw AppException.BadRequest("Must provide a product to delete");
            }
            try
            {
                var deletedProduct = await _productRepo.DeleteProductByIdAsync(productId);

                if (!deletedProduct)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region PATCH
        public async Task<ProductReadDto> UpdateProductByIdAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            try
            {
                if (productId == Guid.Empty)
                {
                    throw AppException.BadRequest("ProductId is required");
                }

                var foundProduct = await _productRepo.GetProductByIdAsync(productId);
                if (foundProduct is null)
                {
                    throw AppException.NotFound("Product not found");
                }

                foundProduct.Description = productUpdateDto.ProductDescription ?? foundProduct.Description;

                if (productUpdateDto.ProductTitle != null && productUpdateDto.ProductTitle != foundProduct.Title)
                {
                    // If updated title length < 3 -> throw exception
                    if (productUpdateDto.ProductTitle.Length < 3)
                    {
                        throw AppException.InvalidInputException("Product title must be at least 3 characters long");
                    }

                    // If updated title is not unique -> throw exception
                    var productList = await _productRepo.GetAllProductsAsync(null);
                    var productTitleExists = productList.FirstOrDefault(p => p.Title == productUpdateDto.ProductTitle);
                    if (productTitleExists != null)
                    {
                        throw AppException.DuplicateProductTitleException("Product title is already in use, please choose another title");
                    }
                    foundProduct.Title = productUpdateDto.ProductTitle;
                }

                if (productUpdateDto.CategoryId.HasValue)
                {
                    var categoryExists = await _categoryRepo.GetCategoryByIdAsync(productUpdateDto.CategoryId.Value);
                    if (categoryExists == null)
                    {
                        throw AppException.BadRequest("Category not found");
                    }
                    foundProduct.CategoryId = productUpdateDto.CategoryId.Value;
                }

                if (productUpdateDto.ProductInventory.HasValue)
                {
                    if (productUpdateDto.ProductInventory < 0)
                    {
                        throw AppException.InvalidInputException("Product inventory cannot be negative");
                    }
                    foundProduct.Inventory = productUpdateDto.ProductInventory.Value;
                }

                if (productUpdateDto.ProductPrice.HasValue)
                {
                    if (productUpdateDto.ProductPrice < 0)
                    {
                        throw AppException.InvalidInputException("Product price cannot be negative");
                    }
                    foundProduct.Price = productUpdateDto.ProductPrice.Value;
                }

                // Update product images (Still not working! -> Tech debt)
                if (productUpdateDto.ImagesToUpdate != null)
                {
                    await _productImageService.UpdateProductImagesAsync(productId, productUpdateDto.ImagesToUpdate);
                }

                // Save changes to the database
                var updatedProduct = await _productRepo.UpdateProductByIdAsync(foundProduct);

                // Fetch category information for the updated product
                var category = await _categoryRepo.GetCategoryByIdAsync(updatedProduct.CategoryId);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);

                // Map the updated product entity to ProductReadDto
                var updatedProductDto = _mapper.Map<Product, ProductReadDto>(updatedProduct);
                updatedProductDto.ProductInventory = foundProduct.Inventory;
                updatedProductDto.Category = categoryDto;
                updatedProductDto.ProductImages = _mapper.Map<IEnumerable<ProductImageReadDto>>(foundProduct.ProductImages);

                return updatedProductDto;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Helper Methods
        bool IsImageDataValid(string ImageData)
        {
            // Regular expression pattern to match common image file extensions (e.g., .jpg, .jpeg, .png, .gif)
            string pattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(jpg|jpeg|gif|png)(\?.*)?$";

            // Create a regular expression object
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Check if the ImageData matches the pattern
            return regex.IsMatch(ImageData);
        }
        #endregion
    }
}