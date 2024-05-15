using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductController : ControllerBase
    {
        #region Properties
        private IProductService _productService;
        #endregion

        #region Constructor
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        #region GET http://localhost:5227/api/v1/products
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProductsAsync([FromQuery] ProductQueryOptions? options)
        {
            var result = await _productService.GetAllProductsAsync(options);
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/products/category/{categoryId}
        [HttpGet("category/{categoryId:guid}")]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProductsByCategoryAsync([FromRoute] Guid categoryId)
        {
            var result = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/products/{productId}
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductReadDto>> GetProductByIdAsync([FromRoute] Guid productId)
        {
            var foundProduct = await _productService.GetProductByIdAsync(productId);
            if (foundProduct is null)
            {
                return NotFound();
            }
            return Ok(foundProduct);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/products
        [Authorize(Roles = "Admin")]
        [HttpPost("")]
        public async Task<ActionResult<ProductReadDto>> CreateProductAsync([FromBody] ProductCreateDto productCreateDto)
        {
            var result = await _productService.CreateProductAsync(productCreateDto);
            return Ok(result);
        }
        #endregion

        #region PATCH http://localhost:5227/api/v1/products/{productId}
        [Authorize(Roles = "Admin")]
        [HttpPatch("{productId}")]
        public async Task<ActionResult<ProductReadDto>> UpdateProductByIdAsync([FromRoute] Guid productId, [FromBody] ProductUpdateDto productUpdateDto)
        {
            var foundProduct = await _productService.GetProductByIdAsync(productId);
            if (foundProduct is null)
            {
                return NotFound();
            }

            var result = await _productService.UpdateProductByIdAsync(productId, productUpdateDto);
            return Ok(result);
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/products/{productId}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId}")]
        public async Task<ActionResult<bool>> DeleteProductByIdAsync([FromRoute] Guid productId)
        {
            var result = await _productService.DeleteProductByIdAsync(productId);
            return Ok(result);
        }
        #endregion
    }
}