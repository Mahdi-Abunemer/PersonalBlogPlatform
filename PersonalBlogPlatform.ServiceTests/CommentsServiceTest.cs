using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Infrastructure.Service;
using PersonalBlogPlatform.ServiceTests.CustomFixtureConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.ServiceTests
{
    public class CommentsServiceTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<PostDomainService> _postDomainServiceMock;
        private readonly Mock<ICommentsRepository> _commentRepositoryMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ICommentService _commentService;
        private readonly Mock<IPostsRepository> _postsRepositoryMock;

        public CommentsServiceTest()
        {
            _fixture = AutoFixtureConfigurator.Create();

            _commentRepositoryMock = new Mock<ICommentsRepository>();
            _mapperMock = new Mock<IMapper>();
            _postsRepositoryMock = new Mock<IPostsRepository>();

            _postDomainServiceMock = new Mock<PostDomainService>(_postsRepositoryMock.Object);

            _userManagerMock = CreateUserManagerMock<ApplicationUser>();

            _mapperMock
                .Setup(m => m.Map<Comment>(It.IsAny<CommentAddRequest>()))
                .Returns((CommentAddRequest req) => new Comment
                {
                    ContentText = req.ContentText,
                    PostId = req.PostId,
                    AuthorId = req.AuthorId ,
                    Post = null!, 
                    Author = null! 
                });

            _mapperMock
                .Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>()))
                .Returns((Comment src) => new CommentResponse
                {
                    Id = src.Id,
                    ContentText = src.ContentText,
                    PostId = src.PostId,
                    AuthorId = src.Author?.Id ?? src.AuthorId
                });

            _commentService = new CommentService(
                _mapperMock.Object,
                _postDomainServiceMock.Object,
                _userManagerMock.Object,
                _commentRepositoryMock.Object
            );
        }

        private static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        #region AddComment

        [Fact]
        public async Task AddComment_WhenRequestIsValid_ReturnsCreatedComment()
        {
            var authorId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var request = _fixture.Build<CommentAddRequest>()
                .With(r => r.AuthorId, authorId)
                .With(r => r.PostId, postId)
                .With(r => r.ContentText, "Nice post!")
                .Create();

            var user = _fixture.Build<ApplicationUser>()
                .With(u => u.Id, authorId)
                .Create();

            _postDomainServiceMock
                .Setup(d => d.EnsureExists(postId))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<Comment>(It.IsAny<CommentAddRequest>()))
                .Returns((CommentAddRequest req) => new Comment
                {
                    ContentText = req.ContentText,
                    PostId = req.PostId,
                    AuthorId = req.AuthorId ,
                    Post = null!,
                    Author = null!
                });

            _userManagerMock
                .Setup(u => u.FindByIdAsync(authorId.ToString()))
                .ReturnsAsync(user);

            Comment captured = null;
            _commentRepositoryMock
                .Setup(r => r.AddComment(It.IsAny<Comment>()))
                .Callback<Comment>(c =>
                {
                    if (c.Id == Guid.Empty) c.Id = Guid.NewGuid();
                    if (c.Author == null) c.Author = user;
                    captured = c;
                })
                .ReturnsAsync(() => captured);

            var result = await _commentService.AddComment(request);

            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.ContentText.Should().Be(request.ContentText);
            result.AuthorId.Should().Be(user.Id);
            result.PostId.Should().Be(postId);

            _postDomainServiceMock.Verify(d => d.EnsureExists(postId), Times.Once);
             _user_manager_verify: _userManagerMock.Verify(u => u.FindByIdAsync(authorId.ToString()), Times.Once);
            _commentRepositoryMock.Verify(r => r.AddComment(It.Is<Comment>(c => c.ContentText == request.ContentText && c.PostId == postId)), Times.Once);
        }

        [Fact]
        public async Task AddComment_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _commentService.AddComment(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddComment_WhenUserNotFound_ThrowsNotFoundException()
        {
            var authorId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            var request = _fixture.Build<CommentAddRequest>()
                .With(r => r.AuthorId, authorId)
                .With(r => r.PostId, postId)
                .With(r => r.ContentText, "Hi")
                .Create();

            _postDomainServiceMock
                .Setup(d => d.EnsureExists(postId))
                .Returns(Task.CompletedTask);

            _userManagerMock
                .Setup(u => u.FindByIdAsync(authorId.ToString()))
                .ReturnsAsync(null as ApplicationUser);

            Func<Task> act = async () => await _commentService.AddComment(request);

            await act.Should().ThrowAsync<NotFoundException>();

            _postDomainServiceMock.Verify(d => d.EnsureExists(postId), Times.Once);
            _userManagerMock.Verify(u => u.FindByIdAsync(authorId.ToString()), Times.Once);
        }

        #endregion

        #region DeleteComment

        [Fact]
        public async Task DeleteComment_WhenCommentExists_DeletesComment()
        {
            var commentId = Guid.NewGuid();

            var comment = _fixture.Build<Comment>()
                .With(c => c.Id, commentId)
                .Create();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(comment);

            _commentRepositoryMock
                .Setup(r => r.DeleteComment(It.Is<Comment>(c => c.Id == commentId)))
                .Returns(Task.CompletedTask);

            Func<Task> act = async () => await _commentService.DeleteComment(commentId);

            await act.Should().NotThrowAsync();

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
            _commentRepositoryMock.Verify(r => r.DeleteComment(It.Is<Comment>(c => c.Id == commentId)), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _commentService.DeleteComment(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task DeleteComment_WhenCommentNotFound_ThrowsNotFoundException()
        {
            var commentId = Guid.NewGuid();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(null as Comment);

            Func<Task> act = async () => await _commentService.DeleteComment(commentId);

            await act.Should().ThrowAsync<NotFoundException>();

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
        }

        #endregion

        #region GetComments

        [Fact]
        public async Task GetAllCommentsByPostId_WhenCommentsExist_ReturnsComments()
        {
            var postId = Guid.NewGuid();

            var comments = _fixture.Build<Comment>()
                .With(c => c.PostId, postId)
                .CreateMany(3)
                .ToList();

            _postDomainServiceMock
                .Setup(d => d.EnsureExists(postId))
                .Returns(Task.CompletedTask);

            _commentRepositoryMock
                .Setup(r => r.GetAllCommentsByPostId(postId))
                .ReturnsAsync(comments);

            _mapperMock
                .Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>()))
                .Returns((Comment src) => new CommentResponse
                {
                    Id = src.Id,
                    ContentText = src.ContentText,
                    PostId = src.PostId,
                    AuthorId = src.Author?.Id ?? src.AuthorId
                });

            var result = await _commentService.GetAllCommentsByPostId(postId);

            result.Should().NotBeNull();
            result.Should().HaveCount(comments.Count);

            _postDomainServiceMock.Verify(d => d.EnsureExists(postId), Times.Once);
            _commentRepositoryMock.Verify(r => r.GetAllCommentsByPostId(postId), Times.Once);
        }

        [Fact]
        public async Task GetCommentByCommentId_WhenCommentExists_ReturnsCommentResponse()
        {
            var commentId = Guid.NewGuid();

            var comment = _fixture.Build<Comment>()
                .With(c => c.Id, commentId)
                .With(c => c.ContentText, "Hello")
                .Create();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(comment);

            _mapperMock
                .Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>()))
                .Returns((Comment src) => new CommentResponse
                {
                    Id = src.Id,
                    ContentText = src.ContentText,
                    PostId = src.PostId,
                    AuthorId = src.Author?.Id ?? src.AuthorId
                });

            var result = await _commentService.GetCommentByCommentId(commentId);

            result.Should().NotBeNull();
            result.Id.Should().Be(commentId);

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
        }

        [Fact]
        public async Task GetCommentByCommentId_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _commentService.GetCommentByCommentId(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task GetCommentByCommentId_WhenCommentNotFound_ThrowsArgumentNullException()
        {
            var commentId = Guid.NewGuid();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(null as Comment);

            Func<Task> act = async () => await _commentService.GetCommentByCommentId(commentId);

            await act.Should().ThrowAsync<ArgumentNullException>();

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
        }

        #endregion

        #region UpdateComment

        [Fact]
        public async Task UpdateComment_WhenRequestIsValid_ReturnsUpdatedComment()
        {
            var commentId = Guid.NewGuid();

            var request = _fixture.Build<CommentUpdateRequest>()
                .With(r => r.Id, commentId)
                .With(r => r.ContentText, "Updated content")
                .Create();

            var existing = _fixture.Build<Comment>()
                .With(c => c.Id, commentId)
                .With(c => c.ContentText, "Old content")
                .Create();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(existing);

            _commentRepositoryMock
                .Setup(r => r.UpdateComment(It.Is<Comment>(c => c.Id == commentId)))
                .Returns(Task.CompletedTask);

            _mapperMock
                .Setup(m => m.Map<CommentResponse>(It.IsAny<Comment>()))
                .Returns((Comment src) => new CommentResponse
                {
                    Id = src.Id,
                    ContentText = src.ContentText,
                    PostId = src.PostId,
                    AuthorId = src.Author?.Id ?? src.AuthorId
                });

            var result = await _commentService.UpdateComment(request);

            result.Should().NotBeNull();
            result.Id.Should().Be(commentId);
            result.ContentText.Should().Be(request.ContentText);

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
            _commentRepositoryMock.Verify(r => r.UpdateComment(It.Is<Comment>(c => c.Id == commentId && c.ContentText == request.ContentText)), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _commentService.UpdateComment(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateComment_WhenIdIsEmpty_ThrowsArgumentException()
        {
            var request = _fixture.Build<CommentUpdateRequest>()
                .With(r => r.Id, Guid.Empty)
                .Create();

            Func<Task> act = async () => await _commentService.UpdateComment(request);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateComment_WhenCommentNotFound_ThrowsArgumentNullException()
        {
            var commentId = Guid.NewGuid();

            var request = _fixture.Build<CommentUpdateRequest>()
                .With(r => r.Id, commentId)
                .With(r => r.ContentText, "Doesn't matter")
                .Create();

            _commentRepositoryMock
                .Setup(r => r.GetCommentByCommentId(commentId))
                .ReturnsAsync(null as Comment);

            Func<Task> act = async () => await _commentService.UpdateComment(request);

            await act.Should().ThrowAsync<ArgumentNullException>();

            _commentRepositoryMock.Verify(r => r.GetCommentByCommentId(commentId), Times.Once);
        }

        #endregion
    }
}
