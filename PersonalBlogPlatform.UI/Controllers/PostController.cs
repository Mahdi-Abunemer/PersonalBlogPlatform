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
        public PostController(IPostsService postsService)
        {
            _postsService = postsService;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAllPosts() 
        {
          var postsResponses = await _postsService.GetAllPosts();

          return Ok(postsResponses);
        }

        [HttpGet]
        [Route("[Action]/{postId:guid}")]
        public async Task<IActionResult> GetPost(Guid postId) 
        {
           var post = await _postsService.GetPostById(postId);

            return Ok(post);
        }

        [HttpGet]
        [Route("[Action]/{categoryId:guid}")]
        public async Task<IActionResult> GetFilteredPosts(Guid categoryId)
        {
            var posts = await _postsService.GetFilteredPosts(categoryId);

            return Ok(posts);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetLatestPosts([FromQuery] int count =5) 
        {
            var posts = await _postsService.GetLatestPosts(count);

            return Ok(posts);
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Add([FromBody] PostAddRequest postAddRequest)
        {
            var post = await _postsService.AddPost(postAddRequest);

            return Ok(post);
        }

        [HttpDelete]
        [Route("[Action]/{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId) 
        {
            await _postsService.DeletePost(postId);

            return Ok();
        }

        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> Update([FromBody] PostUpdateRequest postUpdateRequest)
        {
            var post = await _postsService.UpdatePost(postUpdateRequest);

            return Ok(post);
        }
    }
}
