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
        private readonly IProductRepo _productRepo;
        private IMapper _mapper;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IProductImageRepo _productImageRepo;

        public ProductService(IProductRepo productRepo, IMapper mapper, ICategoryRepo categoryRepo, IProductImageRepo productImageRepo)
        {
            _productRepo = productRepo;
            _mapper = mapper;
            _categoryRepo = categoryRepo;
            _productImageRepo = productImageRepo;
        }

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
                throw AppException.BadRequest();
            }
            try
            {
                var product = await _productRepo.GetProductByIdAsync(productId);
                var productDto = _mapper.Map<ProductReadDto>(product);

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

        public async Task<ProductReadDto> CreateProductAsync(ProductCreateDto newProduct)
        {
            try
            {
                if (newProduct is null)
                {
                    throw AppException.BadRequest("Product cannot be null");
                }

                // Validate image URLs
                if (newProduct.ProductImages is not null)
                {

                    foreach (var image in newProduct.ProductImages)
                    {
                        // Check if the URL is provided
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

        public async Task<ProductReadDto> UpdateProductByIdAsync(Guid productId, ProductUpdateDto productUpdateDto)
        {
            try
            {
                var foundProduct = await _productRepo.GetProductByIdAsync(productId);

                foundProduct.Title = productUpdateDto.ProductTitle ?? foundProduct.Title;
                foundProduct.Description = productUpdateDto.ProductDescription ?? foundProduct.Description;
                foundProduct.CategoryId = productUpdateDto.CategoryId ?? foundProduct.CategoryId;
                foundProduct.Price = productUpdateDto.ProductPrice ?? foundProduct.Price;

                // Update inventory by adding the new inventory value
                if (productUpdateDto.ProductInventory.HasValue)
                {
                    foundProduct.Inventory = productUpdateDto.ProductInventory.Value;
                }

                // Find product images
                var productImages = await _productImageRepo.GetProductImagesByProductIdAsync(productId);

                // Update product images
                if (productUpdateDto.ImagesToUpdate is not null && productUpdateDto.ImagesToUpdate.Any())
                {
                    foreach (var imageDto in productUpdateDto.ImagesToUpdate)
                    {
                        // Find the image to update by ImageData
                        var imageToUpdate = productImages.FirstOrDefault(img => img.ImageData == imageDto.ImageData);

                        if (imageToUpdate is not null)
                        {
                            // Update ImageData if it has changed
                            if (imageToUpdate.ImageData != imageDto.ImageData)
                            {
                                // Update the ImageData using the repository method
                                var updateResult = _productImageRepo.UpdateImageUrlAsync(imageToUpdate.Id, imageDto.ImageData);
                            }
                        }
                        else
                        {
                            // Handle the case where the image URL from the DTO doesn't match any existing images
                            throw AppException.NotFound($"Image with with data {imageDto.ImageData} not found.");
                        }

                        // Validate ImageData
                        if (!IsImageDataValid(imageDto.ImageData))
                        {
                            throw AppException.InvalidInputException("Invalid ImageData format");
                        }
                    }
                }
                // Save changes to the database
                var updatedProduct = await _productRepo.UpdateProductByIdAsync(foundProduct);

                // Fetch category information for the updated product
                var category = await _categoryRepo.GetCategoryByIdAsync(updatedProduct.CategoryId);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);
                // Map the updated product entity to ProductReadDto
                var updatedProductDto = _mapper.Map<Product, ProductReadDto>(updatedProduct);
                // Update the productInventory value in the returned DTO
                updatedProductDto.ProductInventory = foundProduct.Inventory;
                // Set the category property in the updated product DTO
                updatedProductDto.Category = categoryDto;
                return updatedProductDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

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