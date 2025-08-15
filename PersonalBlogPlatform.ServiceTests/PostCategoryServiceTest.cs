using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Infrastructure.Service;
using PersonalBlogPlatform.ServiceTests.CustomFixtureConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.ServiceTests
{
    public class PostCategoryServiceTest
    {
        private readonly IFixture _fixture;
        private readonly IPostCategoryService _postCategoryService;
        private readonly Mock<IPostsRepository> _postsRepositoryMock;
        private readonly Mock<ICategoriesRepository> _categoriesRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
    
        public PostCategoryServiceTest()
        {
            _fixture = AutoFixtureConfigurator.Create();

            _categoriesRepositoryMock = new Mock<ICategoriesRepository>();
            _postsRepositoryMock = new Mock<IPostsRepository>();
            _mapperMock = new Mock<IMapper>();

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title,
                    Content = src.Content
                });

            _postCategoryService = new PostCategoryService(
                                   _postsRepositoryMock.Object,
                                   _categoriesRepositoryMock.Object,
                                   _mapperMock.Object);
        }

        [Fact]
        public async Task AddPostToCategoryAsync_ShouldAddPostToCategory_WhenCategoryAndPostExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .With(c => c.Posts, new List<Post>())
                .Create();

            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _postCategoryService.AddPostToCategoryAsync(categoryId, postId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(postId, "service should map the created post to a PostResponse with same Id");

            category.Posts.Should().ContainSingle(p => p.Id == postId);

            _categoriesRepositoryMock.Verify(repo => repo.UpdateCategory(category), Times.Once);
            _categoriesRepositoryMock.Verify(repo => repo.GetCategoryByCategoryId(categoryId), Times.Once);
            _postsRepositoryMock.Verify(repo => repo.GetPostByPostId(postId), Times.Once);
            _mapperMock.Verify(m => m.Map<PostResponse>(post), Times.Once);
        }

        [Fact]
        public async Task AddPostToCategoryAsync_ShouldThrowInvalidOperationException_WhenPostAlreadyExistsInCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .With(c => c.Posts, new List<Post> { new Post { Id = postId } })
                .Create();


            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync(post);

            // Act
            Func<Task> act = async () => await _postCategoryService.AddPostToCategoryAsync(categoryId, postId);

            await act.Should().ThrowAsync<InvalidOperationException>();
               
        }

        [Fact]
        public async Task AddPostToCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync((Category)null);

            // Act
            Func<Task> act = async () => await _postCategoryService.AddPostToCategoryAsync(categoryId, postId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task AddPostToCategoryAsync_ShouldThrowNotFoundException_WhenPostDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .With(c => c.Posts, new List<Post>())
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync((Post)null);

            // Act
            Func<Task> act = async () => await _postCategoryService.AddPostToCategoryAsync(categoryId, postId);
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RemovePostFromCategoryAsync_ShouldRemovePostFromCategory_WhenCategoryAndPostExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .With(c => c.Posts, new List<Post> { new Post { Id = postId } })
                .Create();

            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync(post);

            _categoriesRepositoryMock.Setup(repo => repo.UpdateCategory(category))
                .Returns(Task.CompletedTask);

            // Act
            await _postCategoryService.RemovePostFromCategoryAsync(categoryId, postId);

            // Assert
            category.Posts.Should().NotContain(p => p.Id == postId);

            _categoriesRepositoryMock.Verify(repo => repo.UpdateCategory(category), Times.Once);

            _categoriesRepositoryMock.Verify(repo => repo.GetCategoryByCategoryId(categoryId), Times.Once);

            _postsRepositoryMock.Verify(repo => repo.GetPostByPostId(postId), Times.Once);
        }

        [Fact]
        public async Task RemovePostFromCategoryAsync_ShouldThrowNotFoundException_WhenPostIsNotAssociatedWithCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync(post);

            _categoriesRepositoryMock.Setup(repo => repo.UpdateCategory(category))
               .Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _postCategoryService.RemovePostFromCategoryAsync(categoryId, postId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RemovePostFromCategoryAsync_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync((Category)null);

            // Act
            Func<Task> act = async () => await _postCategoryService.RemovePostFromCategoryAsync(categoryId, postId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task RemovePostFromCategoryAsync_ShouldThrowNotFoundException_WhenPostDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            _categoriesRepositoryMock.Setup(repo => repo.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock.Setup(repo => repo.GetPostByPostId(postId))
                .ReturnsAsync((Post)null);

            // Act
            Func<Task> act = async () => await _postCategoryService.RemovePostFromCategoryAsync(categoryId, postId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}