using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.ServiceContracts;

namespace PersonalBlogPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        [Route("[Action]/{postId:guid}")]
        public async Task<ActionResult<List<CommentResponse>>> GetAllCommentsByPostId(Guid postId)
        {
            List<CommentResponse> comments = await _commentService.GetAllCommentsByPostId(postId);

            return Ok(comments);
        }

        [HttpGet]
        [Route("[Action]/{commentId:guid}")]
        public async Task<ActionResult<CommentResponse>> GetCommentByCommentId(Guid commentId)
        {
            var comment = await _commentService.GetCommentByCommentId(commentId);

            return Ok(comment);
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<ActionResult<CommentResponse>> AddComment([FromBody] CommentAddRequest commentAddRequest)
        {
            var comment = await _commentService.AddComment(commentAddRequest);

            return Ok(comment);
        }

        [HttpDelete]
        [Route("[Action]/{commentId:guid}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            await _commentService.DeleteComment(commentId);

            return Ok();
        }

        [HttpPut]
        [Route("[Action]")]
        public async Task<ActionResult<CommentResponse>> UpdateComment([FromBody] CommentUpdateRequest request)
        {
            var updatedComment = await _commentService.UpdateComment(request);

            return Ok(updatedComment);
        }
    }
}
