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
        private readonly IProductImageRepo _productImageRepo;
        private readonly IProductImageService _productImageService;
        #endregion

        #region Constructors
        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo, IProductImageRepo productImageRepo, IProductImageService productImageService)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _productImageRepo = productImageRepo;
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
                if (newProduct is null)
                {
                    throw AppException.BadRequest("Product cannot be null");
                }

                // Validate image data
                if (newProduct.ProductImages is not null)
                {
                    foreach (var image in newProduct.ProductImages)
                    {
                        // Check if the image data is provided
                        if (string.IsNullOrWhiteSpace(image.ImageData))
                        {
                            throw AppException.InvalidInputException("Image Data cannot be empty");
                        }

                        // Check if the ImageData points to a valid image format 
                        if (!IsImageDataValid(image.ImageData))
                        {
                            throw AppException.InvalidInputException("Invalid image format");
                        }
                    }
                }
                // Check if the specified category ID exists
                var category = await _categoryRepo.GetCategoryByIdAsync(newProduct.CategoryId);
                if (category == null)
                {
                    throw AppException.NotFound("Category not found");
                }
                var productEntity = _mapper.Map<Product>(newProduct);
                productEntity.ProductImages = _mapper.Map<List<ProductImage>>(newProduct.ProductImages);
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

                foundProduct.Title = productUpdateDto.ProductTitle ?? foundProduct.Title;
                foundProduct.Description = productUpdateDto.ProductDescription ?? foundProduct.Description;
                foundProduct.CategoryId = productUpdateDto.CategoryId ?? foundProduct.CategoryId;
                foundProduct.Price = productUpdateDto.ProductPrice ?? foundProduct.Price;

                if (productUpdateDto.ProductInventory.HasValue)
                {
                    foundProduct.Inventory = productUpdateDto.ProductInventory.Value;
                }

                // Update product images
                if (productUpdateDto.ImagesToUpdate != null)
                {
                    foundProduct.ProductImages = _mapper.Map<List<ProductImage>>(productUpdateDto.ImagesToUpdate);
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

        // Method to validate image URL
        bool IsImageDataValid(string ImageData)
        {
            // Regular expression pattern to match common image file extensions (e.g., .jpg, .jpeg, .png, .gif)
            string pattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|jpeg|gif|png)$";

            // Create a regular expression object
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Check if the ImageData matches the pattern
            return regex.IsMatch(ImageData);
        }
    }
}