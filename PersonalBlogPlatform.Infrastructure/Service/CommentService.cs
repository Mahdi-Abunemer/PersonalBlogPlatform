using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.Helper;
using PersonalBlogPlatform.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Infrastructure.Service
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly PostDomainService _postDomain;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommentsRepository _commentsRepository;

        public CommentService(
            IMapper mapper,
            PostDomainService postDomain,
            UserManager<ApplicationUser> users,
            ICommentsRepository commentsRepo)
        {
            _mapper = mapper;
            _postDomain = postDomain;
            _userManager = users;
            _commentsRepository = commentsRepo;
        }


        public async Task<CommentResponse> AddComment(CommentAddRequest commentAddRequest)
        {
           if(commentAddRequest == null) 
                throw new ArgumentNullException("Comment shouldn't be empty" , nameof(commentAddRequest));

           ValidationHelper.ModelValidation(commentAddRequest);

           await _postDomain.EnsureExists(commentAddRequest.PostId);

           var comment = _mapper.Map<Comment>(commentAddRequest);

           comment.Id = Guid.NewGuid();
            
            
           comment.Author = await _userManager.FindByIdAsync(commentAddRequest.AuthorId.ToString())
                         ?? throw new NotFoundException($"User {commentAddRequest.AuthorId} not found");

           await _commentsRepository.AddComment(comment);
            
           return _mapper.Map<CommentResponse>(comment);
        }

        public async Task DeleteComment(Guid commentId)
        {
            if (commentId == Guid.Empty)
                throw new InvalidIDException($"Comment ID:{commentId} cannot be empty");

            var comment =  await _commentsRepository.GetCommentByCommentId(commentId);
            if (comment == null)
                throw new NotFoundException($"Comment: {commentId} is not found");

            await _commentsRepository.DeleteComment(comment);
        }

        public async Task<List<CommentResponse>> GetAllCommentsByPostId(Guid postId)
        {
            await _postDomain.EnsureExists(postId);

            var comments = await _commentsRepository.GetAllCommentsByPostId(postId);

            return comments
               .Select(comment => _mapper.Map<CommentResponse>(comment))
               .ToList();
        }

        public async Task<CommentResponse> GetCommentByCommentId(Guid commentId)
        {
            if (commentId == Guid.Empty)
                throw new InvalidIDException($"Comment ID:{commentId} cannot be empty");

            var comment = await _commentsRepository.GetCommentByCommentId(commentId);
            if (comment == null)
                throw new ArgumentNullException(nameof(comment), $"Comment {commentId} not found");

            return _mapper.Map<CommentResponse>(comment);
        }

        public async Task<CommentResponse> UpdateComment(CommentUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Comment ID cannot be empty", nameof(request.Id));

            ValidationHelper.ModelValidation(request);

            var comment = await _commentsRepository.GetCommentByCommentId(request.Id);
            if (comment == null)
                throw new ArgumentNullException(nameof(comment), $"Comment {request.Id} not found");

           comment.ContentText = request.ContentText;
           
            await _commentsRepository.UpdateComment(comment);

            return _mapper.Map<CommentResponse>(comment);
        }
    }
}
