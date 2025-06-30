using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Helper;
using PersonalBlogPlatform.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository _postsRepository;

        public PostsService(IPostsRepository postsRepository)
        {
            _postsRepository = postsRepository;
        }

        private static List<PostResponse> ToListPostResponse(IEnumerable<Post> posts)
            => posts.Select(p => p.ToPostResponse()).ToList();

        public async Task<PostResponse> AddPost(PostAddRequest postAddRequest)
        {
           if (postAddRequest == null) 
                throw new ArgumentNullException(nameof(postAddRequest));

            ValidationHelper.ModelValidation(postAddRequest);

           Post post = postAddRequest.ToPost();

            post.Id = Guid.NewGuid();

            await _postsRepository.AddPost(post);

            return post.ToPostResponse();
        }

        public async Task<bool> DeletePostByPostId(Guid postId)
        {
            var deleted = await _postsRepository.DeletePostByPostId(postId);

            if (!deleted)
                throw new ArgumentNullException($"Post {postId} not found");

            return true;
        }

        public async Task<List<PostResponse>> GetAllPosts()
        {
          List<Post> posts = await _postsRepository.GetAllPosts();

          return ToListPostResponse(posts);
        }

        public async Task<List<PostResponse>> GetFilteredPosts(Guid categoryId)
        {
            //TO DO : Verifying that the category exist in the data store
            //(to add a method here from => "CategoryRepo")
            List<Post> posts  = await _postsRepository.GetFilteredPosts(categoryId);
         
            return ToListPostResponse(posts);
        }
      
        public async Task<List<PostResponse>> GetLatestPosts(int count = 5)
        {
            List<Post> posts = await _postsRepository.GetLatestPosts(count);

            return ToListPostResponse(posts);
        }

        public async Task<PostResponse?> GetPostById(Guid postId)
        {
            Post? post = await _postsRepository.GetPostByPostId(postId);

            if (post == null)
                throw new ArgumentNullException(nameof(post));

            return post.ToPostResponse();
        }

        public async Task<PostResponse?> UpdatePost(PostUpdateRequest postUpdateRequest)
        {
            if (postUpdateRequest == null)
                throw new ArgumentNullException(nameof(postUpdateRequest));

            ValidationHelper.ModelValidation(postUpdateRequest);

            Post? post = await _postsRepository.GetPostByPostId(postUpdateRequest.Id);

            if (post == null)
                throw new ArgumentNullException($"Post {postUpdateRequest.Id} not found");


            post.UpdatedAt = DateTime.UtcNow;
            post.Title = postUpdateRequest.Title;
            post.PostDetails = postUpdateRequest.PostDetails;
            post.Content = postUpdateRequest.Content;
            post.IsPublished = postUpdateRequest.IsPublished;

            Post? postUpdated =  await _postsRepository.UpdatePost(post);
            
            return postUpdated?.ToPostResponse();
        }
    }
}
