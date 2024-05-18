using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.RepoAbstract;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;

namespace Ecommerce.Service.src.Service
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepo _productImageRepo;
        private readonly IMapper _mapper;
        public ProductImageService(IProductImageRepo productImageRepo, IMapper mapper)
        {
            _productImageRepo = productImageRepo;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductImageReadDto>> GetProductImagesByProductIdAsync(Guid productId)
        {
            var productImages = await _productImageRepo.GetProductImagesByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductImageReadDto>>(productImages);
        }

        public async Task UpdateProductImagesAsync(Guid productId, IEnumerable<ProductImageUpdateDto> imagesToUpdate)
        {
            var productImages = await _productImageRepo.GetProductImagesByProductIdAsync(productId);
            if (productImages is null)
            {
                throw AppException.NotFound("Product images not found");
            }

            foreach (var image in imagesToUpdate)
            {
                var foundImage = productImages.FirstOrDefault(i => i.Id == image.ImageId);
                if (foundImage is null)
                {
                    throw AppException.NotFound($"Image with ID {image.ImageId} not found");
                }
                // Validate Image Data by IsImageDataValid
                if (!IsImageDataValid(image.ImageData))
                {
                    throw AppException.InvalidInputException("Invalid image data");
                }
                foundImage.ImageData = image.ImageData;
                foundImage.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

                await _productImageRepo.UpdateImageDataAsync(foundImage.Id, image.ImageData);
            }
        }

        bool IsImageDataValid(string ImageData)
        {
            // Regular expression pattern to match common image file extensions (e.g., .jpg, .jpeg, .png, .gif)
            string pattern = @"^(http(s?):)([/|.|\w|\s|-])*\.(jpg|jpeg|gif|png)(\?.*)?$";

            // Create a regular expression object
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            // Check if the ImageData matches the pattern
            return regex.IsMatch(ImageData);
        }

    }
}