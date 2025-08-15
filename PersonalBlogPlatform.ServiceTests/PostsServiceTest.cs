using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Moq;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.Service;
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
    public class PostsServiceTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<ICategoriesRepository> _categoryRepositoryMock;
        private readonly Mock<IPostsRepository> _postsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IPostsService _postsService;

        public PostsServiceTest()
        {
            _fixture = AutoFixtureConfigurator.Create();

            _categoryRepositoryMock = new Mock<ICategoriesRepository>();
            _postsRepositoryMock = new Mock<IPostsRepository>();

            _mapperMock = new Mock<IMapper>();
            _mapperMock
                .Setup(m => m.Map<Post>(It.IsAny<PostAddRequest>()))
                .Returns((PostAddRequest req) => new Post
                {
                });

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                   Content = src.Content,
                   Title = src.Title
                });

            _postsService = new PostsService(_postsRepositoryMock.Object, _categoryRepositoryMock.Object,
                                              _mapperMock.Object);
        }

        #region Add
        [Fact]
        public async Task AddPost_WhenRequestIsValid_ReturnsCreatedPost()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            var request = _fixture.Build<PostAddRequest>()
                .With(r => r.Title, "Test title")
                .With(r => r.Content, "Test content")
                .With(r => r.CategoryIds, new List<Guid> { categoryId })
                .Create();

            _mapperMock
                .Setup(m => m.Map<Post>(It.IsAny<PostAddRequest>()))
                .Returns((PostAddRequest req) => new Post
                {
                    Title = req.Title,
                    Content = req.Content,
                    Categories = new List<Category>()
                });

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title,
                    Content = src.Content,
                    CreatedAt = src.CreatedAt,
                    UpdatedAt = src.UpdatedAt,
                    IsPublished = src.IsPublished,
                    AuthorId = src.AuthorId,
                    CommentIds = src.Comments?.Select(c => c.Id).ToList(),
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            _categoryRepositoryMock
                .Setup(r => r.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock
                .Setup(r => r.AddPost(It.IsAny<Post>()))
                .ReturnsAsync((Post p) => p);

            var result = await _postsService.AddPost(request, userId);

            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.AuthorId.Should().Be(userId);
            result.Title.Should().Be(request.Title);
            result.Content.Should().Be(request.Content);
            result.CategoryIds.Should().NotBeNull().And.HaveCount(1);
            result.CategoryIds.Should().Contain(categoryId);

            _categoryRepositoryMock.Verify(r => r.GetCategoryByCategoryId(categoryId), Times.Once);
            _postsRepositoryMock.Verify(r => r.AddPost(It.Is<Post>(p => p.AuthorId == userId && p.Categories.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task AddPost_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _postsService.AddPost(null , Guid.NewGuid());

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Delete
        [Fact]
        public async Task DeletePost_WhenPostExists_DeletesPost()
        {
            var postId = Guid.NewGuid();

            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .Create();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(post);

            _postsRepositoryMock
                .Setup(r => r.DeletePost(It.Is<Post>(p => p.Id == postId)))
                .Returns(Task.CompletedTask);

            Func<Task> act = async () => await _postsService.DeletePost(postId);

            await act.Should().NotThrowAsync();

            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
            _postsRepositoryMock.Verify(r => r.DeletePost(It.Is<Post>(p => p.Id == postId)), Times.Once);
        }

        [Fact]
        public async Task DeletePost_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _postsService.DeletePost(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task DeletePost_WhenPostNotFound_ThrowsNotFoundException()
        {
            var postId = Guid.NewGuid();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(null as Post);

            Func<Task> act = async () => await _postsService.DeletePost(postId);

            await act.Should().ThrowAsync<NotFoundException>();

            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
        }

        #endregion

        #region Get

        [Fact]
        public async Task GetAllPosts_WhenPostsExist_ReturnsAllPosts()
        {
            // Arrange
            var posts = _fixture.Build<Post>()
                .With(p => p.Title, "Title")
                .With(p => p.Content, "Content")
                .CreateMany(3)
                .ToList();

            _postsRepositoryMock
                .Setup(r => r.GetAllPosts())
                .ReturnsAsync(posts);

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title ?? string.Empty,
                    Content = src.Content ?? string.Empty,
                    AuthorId = src.AuthorId,
                    CreatedAt = src.CreatedAt,
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            // Act
            var result = await _postsService.GetAllPosts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(posts.Count);
            _postsRepositoryMock.Verify(r => r.GetAllPosts(), Times.Once);
        }

        [Fact]
        public async Task GetFilteredPosts_WhenCategoryExists_ReturnsPosts()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            var posts = _fixture.Build<Post>()
                .With(p => p.Categories, new List<Category> { category })
                .CreateMany(2)
                .ToList();

            _categoryRepositoryMock
                .Setup(r => r.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _postsRepositoryMock
                .Setup(r => r.GetFilteredPosts(categoryId))
                .ReturnsAsync(posts);

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title ?? string.Empty,
                    Content = src.Content ?? string.Empty,
                    AuthorId = src.AuthorId,
                    CreatedAt = src.CreatedAt,
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            // Act
            var result = await _postsService.GetFilteredPosts(categoryId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(posts.Count);
            _categoryRepositoryMock.Verify(r => r.GetCategoryByCategoryId(categoryId), Times.Once);
            _postsRepositoryMock.Verify(r => r.GetFilteredPosts(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetFilteredPosts_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            // Arrange
            Func<Task> act = async () => await _postsService.GetFilteredPosts(Guid.Empty);

            // Assert
            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task GetFilteredPosts_WhenCategoryNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(r => r.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(null as Category);

            Func<Task> act = async () => await _postsService.GetFilteredPosts(categoryId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _categoryRepositoryMock.Verify(r => r.GetCategoryByCategoryId(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetLatestPosts_WhenCountValid_ReturnsLatestPosts()
        {
            // Arrange
            var count = 2;

            var posts = _fixture.Build<Post>()
                .With(p => p.CreatedAt, DateTime.UtcNow)
                .CreateMany(count)
                .ToList();

            _postsRepositoryMock
                .Setup(r => r.GetLatestPosts(count))
                .ReturnsAsync(posts);

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title ?? string.Empty,
                    Content = src.Content ?? string.Empty,
                    AuthorId = src.AuthorId,
                    CreatedAt = src.CreatedAt,
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            // Act
            var result = await _postsService.GetLatestPosts(count);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(posts.Count);
            _postsRepositoryMock.Verify(r => r.GetLatestPosts(count), Times.Once);
        }

        [Fact]
        public async Task GetLatestPosts_WhenCountInvalid_ThrowsArgumentException()
        {
            // Arrange
            Func<Task> act = async () => await _postsService.GetLatestPosts(0);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetPostById_WhenPostExists_ReturnsPostResponse()
        {
            // Arrange
            var postId = Guid.NewGuid();

            var post = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .With(p => p.Title, "Title")
                .With(p => p.Content, "Content")
                .Create();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(post);

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title ?? string.Empty,
                    Content = src.Content ?? string.Empty,
                    AuthorId = src.AuthorId,
                    CreatedAt = src.CreatedAt,
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            // Act
            var result = await _postsService.GetPostById(postId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(postId);
            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
        }

        [Fact]
        public async Task GetPostById_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _postsService.GetPostById(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task GetPostById_WhenPostNotFound_ThrowsArgumentNullException()
        {
            var postId = Guid.NewGuid();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(null as Post);

            Func<Task> act = async () => await _postsService.GetPostById(postId);

            await act.Should().ThrowAsync<ArgumentNullException>();
            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdatePost_WhenRequestIsValid_ReturnsUpdatedPost()
        {
            var postId = Guid.NewGuid();

            var request = _fixture.Build<PostUpdateRequest>()
                .With(r => r.Id, postId)
                .With(r => r.Title, "Updated title")
                .With(r => r.Content, "Updated content")
                .With(r => r.PostDetails, "Updated details")
                .With(r => r.IsPublished, true)
                .Create();

            var existingPost = _fixture.Build<Post>()
                .With(p => p.Id, postId)
                .With(p => p.Title, "Old title")
                .With(p => p.Content, "Old content")
                .With(p => p.IsPublished, false)
                .Create();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(existingPost);

            _postsRepositoryMock
                .Setup(r => r.UpdatePost(It.Is<Post>(p => p.Id == postId)))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<PostResponse>(It.IsAny<Post>()))
                .Returns((Post src) => new PostResponse
                {
                    Id = src.Id,
                    Title = src.Title ?? string.Empty,
                    Content = src.Content ?? string.Empty,
                    PostDetails = src.PostDetails,
                    CreatedAt = src.CreatedAt,
                    UpdatedAt = src.UpdatedAt,
                    IsPublished = src.IsPublished,
                    AuthorId = src.AuthorId,
                    CategoryIds = src.Categories?.Select(c => c.Id).ToList()
                });

            var result = await _postsService.UpdatePost(request);

            result.Should().NotBeNull();
            result.Id.Should().Be(postId);
            result.Title.Should().Be(request.Title);
            result.Content.Should().Be(request.Content);
            result.PostDetails.Should().Be(request.PostDetails);
            result.IsPublished.Should().Be(request.IsPublished);

            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
            _postsRepositoryMock.Verify(r => r.UpdatePost(It.Is<Post>(p => p.Id == postId && p.Title == request.Title && p.Content == request.Content)), Times.Once);
        }

        [Fact]
        public async Task UpdatePost_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _postsService.UpdatePost(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePost_WhenIdIsEmpty_ThrowsArgumentException()
        {
            var request = _fixture.Build<PostUpdateRequest>()
                .With(r => r.Id, Guid.Empty)
                .Create();

            Func<Task> act = async () => await _postsService.UpdatePost(request);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePost_WhenPostNotFound_ThrowsNotFoundException()
        {
            var postId = Guid.NewGuid();

            var request = _fixture.Build<PostUpdateRequest>()
                .With(r => r.Id, postId)
                .With(r => r.Title, "Doesn't matter")
                .With(r => r.Content, "Doesn't matter")
                .Create();

            _postsRepositoryMock
                .Setup(r => r.GetPostByPostId(postId))
                .ReturnsAsync(null as Post);

            Func<Task> act = async () => await _postsService.UpdatePost(request);

            await act.Should().ThrowAsync<NotFoundException>();

            _postsRepositoryMock.Verify(r => r.GetPostByPostId(postId), Times.Once);
        }

        #endregion

    }
}
