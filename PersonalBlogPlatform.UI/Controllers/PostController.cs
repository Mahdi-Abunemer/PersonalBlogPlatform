using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.ServiceContracts;

namespace PersonalBlogPlatform.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostsService _postsService;
        private readonly IProfileService _profileService;
        public PostController(IPostsService postsService, IProfileService profileService)
        {
            _postsService = postsService;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostResponse>>> GetAllPosts() 
        {
          var postsResponses = await _postsService.GetAllPosts();

          return Ok(postsResponses);
        }

        [HttpGet]
        [Route("[Action]/{postId:guid}")]
        public async Task<ActionResult<PostResponse>> GetPost(Guid postId) 
        {
           var post = await _postsService.GetPostById(postId);

            return Ok(post);
        }

        [HttpGet]
        [Route("[Action]/{categoryId:guid}")]
        public async Task<ActionResult<List<PostResponse>>> GetFilteredPosts(Guid categoryId)
        {
            var posts = await _postsService.GetFilteredPosts(categoryId);

            return Ok(posts);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[Action]")]
        public async Task<ActionResult<List<PostResponse>>> GetLatestPosts([FromQuery] int count =5) 
        {
            var posts = await _postsService.GetLatestPosts(count);

            return Ok(posts);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [Route("[Action]")]
        public async Task<ActionResult<PostResponse>> Add([FromBody] PostAddRequest postAddRequest)
        {
            var user = await _profileService.GetCurrentUserAsync();
            var post = await _postsService.AddPost(postAddRequest , user.Id);

            return Ok(post);
        }
        
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        [Route("[Action]/{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId) 
        {
            await _postsService.DeletePost(postId);

            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut]
        [Route("[Action]")]
        public async Task<ActionResult<PostResponse>> Update([FromBody] PostUpdateRequest postUpdateRequest)
        {
            var post = await _postsService.UpdatePost(postUpdateRequest);

            return Ok(post);
        }
    }
}
