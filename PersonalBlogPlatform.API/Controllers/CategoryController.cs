using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.ServiceContracts;

namespace PersonalBlogPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
        private readonly IPostCategoryService _postCategoryService;

        public CategoryController(IPostCategoryService postCategoryService, ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
            _postCategoryService = postCategoryService;
        }

        [HttpGet]
        [Route("[Action]/{categoryId:guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(Guid categoryId)
        {
           var category = await _categoriesService.GetCategoryByCategoryId(categoryId);

            return Ok(category);
        }

        [HttpPost]
        [Route("[Action]")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryResponse>> AddCategory([FromBody] CategoryAddRequest categoryAddRequest)
        {
            var category = await _categoriesService.AddCategory(categoryAddRequest);

            return Ok(category);
        }

        [HttpDelete]
        [Route("[Action]/{categoryId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            await _categoriesService.DeleteCategory(categoryId);

            return Ok();
        }

        [HttpPut]
        [Route("[Action]")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromBody] CategoryUpdateRequest categoryUpdateRequest)
        {
            var updatedCategory = await _categoriesService.UpdateCategory(categoryUpdateRequest);

            return Ok(updatedCategory);
        }

        [HttpPost]
        [Route("[Action]/{categoryId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<PostResponse>> AddPostToCategory(Guid categoryId,[FromQuery] Guid postId)
        {
            var post = await _postCategoryService.AddPostToCategoryAsync(categoryId, postId);

            return Ok(post);
        }

        [HttpDelete]
        [Route("[Action]/{categoryId:guid}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RemovePostFromCategory(Guid categoryId,[FromQuery] Guid postId)
        {
            await _postCategoryService.RemovePostFromCategoryAsync(categoryId, postId);

            return NoContent();
        }
    }
}
