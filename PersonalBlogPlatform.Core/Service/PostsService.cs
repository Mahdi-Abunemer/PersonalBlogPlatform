using AutoMapper;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Helper;
using PersonalBlogPlatform.Core.ServiceContracts;

namespace PersonalBlogPlatform.Core.Service
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;

        public PostsService(
            IPostsRepository postsRepository,
            ICategoriesRepository categoriesRepository,
            IMapper mapper)
        {
            _postsRepository = postsRepository;
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
        }

        private List<PostResponse> ToListPostResponse(IEnumerable<Post> posts)
        {
            return posts
                .Select(post => _mapper.Map<PostResponse>(post))
                .ToList();
        }

        public async Task<PostResponse> AddPost(PostAddRequest postAddRequest)
        {
            if (postAddRequest == null)
                throw new ArgumentNullException(nameof(postAddRequest));

            ValidationHelper.ModelValidation(postAddRequest);

            var post = _mapper.Map<Post>(postAddRequest);

            post.Id = Guid.NewGuid();
            post.Categories = new List<Category>();

            if (postAddRequest.CategoryIds?.Any() == true)
            {
                foreach (var categoryId in postAddRequest.CategoryIds)
                {
                    var category = await _categoriesRepository.GetCategoryByCategoryId(categoryId);
                    if (category == null)
                        throw new ArgumentNullException($"Category {categoryId} not found");

                    post.Categories.Add(category);
                }
            }

            await _postsRepository.AddPost(post);

            return _mapper.Map<PostResponse>(post);
        }

        public async Task DeletePost(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException("Post ID cannot be empty", nameof(postId));

            var post = await _postsRepository.GetPostByPostId(postId);
            if (post == null)
                throw new ArgumentNullException(nameof(post), $"Post {postId} not found");

            await _postsRepository.DeletePost(post);
        }

        public async Task<List<PostResponse>> GetAllPosts()
        {
            var posts = await _postsRepository.GetAllPosts();
            return ToListPostResponse(posts);
        }

        public async Task<List<PostResponse>> GetFilteredPosts(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty", nameof(categoryId));

            var category = await _categoriesRepository.GetCategoryByCategoryId(categoryId);
            if (category == null)
                throw new ArgumentNullException(nameof(category), $"Category {categoryId} not found");

            var posts = await _postsRepository.GetFilteredPosts(categoryId);
            return ToListPostResponse(posts);
        }

        public async Task<List<PostResponse>> GetLatestPosts(int count = 5)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero", nameof(count));

            var posts = await _postsRepository.GetLatestPosts(count);
            return ToListPostResponse(posts);
        }

        public async Task<PostResponse> GetPostById(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException("Post ID cannot be empty", nameof(postId));

            var post = await _postsRepository.GetPostByPostId(postId);
            if (post == null)
                throw new ArgumentNullException(nameof(post), $"Post {postId} not found");

            return _mapper.Map<PostResponse>(post);
        }

        public async Task<PostResponse> UpdatePost(PostUpdateRequest postUpdateRequest)
        {
            if (postUpdateRequest == null)
                throw new ArgumentNullException(nameof(postUpdateRequest));

            if (postUpdateRequest.Id == Guid.Empty)
                throw new ArgumentException("Post ID cannot be empty", nameof(postUpdateRequest.Id));

            ValidationHelper.ModelValidation(postUpdateRequest);

            var post = await _postsRepository.GetPostByPostId(postUpdateRequest.Id);
            if (post == null)
                throw new ArgumentNullException(nameof(post), $"Post {postUpdateRequest.Id} not found");

            post.UpdatedAt = DateTime.UtcNow;
            post.Title = postUpdateRequest.Title!;
            post.PostDetails = postUpdateRequest.PostDetails;
            post.Content = postUpdateRequest.Content!;
            post.IsPublished = postUpdateRequest.IsPublished;

            await _postsRepository.UpdatePost(post);

            return _mapper.Map<PostResponse>(post);
        }
    }
}
