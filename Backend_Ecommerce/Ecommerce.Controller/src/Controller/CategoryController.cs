using Ecommerce.Service.src.DTO;
using Ecommerce.Service.src.ServiceAbstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controller
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        #region Properties
        private readonly ICategoryService _categoryService;
        #endregion

        #region Constructor
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        #endregion

        #region GET http://localhost:5227/api/v1/categories
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }
        #endregion

        #region GET http://localhost:5227/api/v1/categories/:category_id
        [AllowAnonymous]
        [HttpGet("{categoryId:guid}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryByIdAsync([FromRoute] Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(result);
        }
        #endregion

        #region POST http://localhost:5227/api/v1/categories
        [Authorize(Roles = "Admin")]
        [HttpPost] // endpoint: /categories
        public async Task<ActionResult<CategoryReadDto>> CreateCategoryAsync([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryCreateDto);
            return Ok(result);
        }
        #endregion

        #region PATCH http://localhost:5227/api/v1/categories/:category_id
        [Authorize(Roles = "Admin")]
        [HttpPatch("{categoryId}")]
        public async Task<ActionResult<CategoryReadDto>> UpdateCategoryByIdAsync([FromRoute] Guid categoryId, [FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            var foundCategory = await GetCategoryByIdAsync(categoryId);
            if (foundCategory is null)
            {
                return NotFound("Category not found");
            }

            var result = await _categoryService.UpdateCategoryByIdAsync(categoryId, categoryUpdateDto);
            return Ok(result);
        }
        #endregion

        #region DELETE http://localhost:5227/api/v1/categories/:category_id
        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<bool>> DeleteCategoryByIdAsync([FromRoute] Guid categoryId)
        {
            var foundCategory = await GetCategoryByIdAsync(categoryId);
            if (foundCategory is null)
            {
                return NotFound("Category not found");
            }

            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);
            return Ok(result);
        }
        #endregion
    }
}