using AutoMapper;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Infrastructure.Service
{
    public class PostCategoryService : IPostCategoryService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IMapper _mapper;

        public PostCategoryService(IPostsRepository postsRepository , ICategoriesRepository categoriesRepository , IMapper mapper) 
        {
            _postsRepository = postsRepository;
            _categoriesRepository = categoriesRepository;
            _mapper = mapper;
        }

        public async Task<PostResponse> AddPostToCategoryAsync(Guid categoryId, Guid postId)
        {
            var category = await EnsureCategoryExist(categoryId);

            var post = await EnsurePostExist(postId);
            
            if (category.Posts!.Any(p => p.Id == postId))
                throw new InvalidOperationException($"Post: {postId} is already associated with Category: {categoryId}");

            category.Posts!.Add(post);
          
            await _categoriesRepository.UpdateCategory(category);

            return _mapper.Map<PostResponse>(post);
        }

        public async Task RemovePostFromCategoryAsync(Guid categoryId, Guid postId)
        {
            var category = await EnsureCategoryExist(categoryId);

            var post = await EnsurePostExist(postId);

            HasPosts(category);

            if (!category.Posts!.Any(p => p.Id == postId))
                throw new NotFoundException($"Post: {postId} is not associated with Category: {categoryId}");

             category.Posts!.Remove(post);
          
            await _categoriesRepository.UpdateCategory(category);
        }

        private async Task<Post> EnsurePostExist(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new InvalidIDException($"Post ID:{postId} cannot be empty");

            var post = await _postsRepository.GetPostByPostId(postId);
            if (post == null)
                throw new NotFoundException($"Post: {postId} is not found");

            return post;
        }

        private async Task<Category> EnsureCategoryExist(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new InvalidIDException($"Category ID:{categoryId} cannot be empty");

            var category = await _categoriesRepository.GetCategoryByCategoryId(categoryId);
            if (category == null)
                throw new NotFoundException($"Category: {categoryId} is not found");

            return category;
        }
        
        private void HasPosts(Category category)
        {
            if (category.Posts == null || !category.Posts.Any())
                throw new InvalidOperationException(
                    $"Category '{category.Id}' has no associated posts.");
        }
    }
}
