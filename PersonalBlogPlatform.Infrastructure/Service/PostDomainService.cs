using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Infrastructure.Service
{
    public class PostDomainService
    {
        private readonly IPostsRepository _postRepository;
        
        public PostDomainService(IPostsRepository repo) 
        { 
            _postRepository = repo; 
        }

        public async Task EnsureExists(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new InvalidIDException($"Post ID:{postId} cannot be empty");

            var exists = await _postRepository.GetPostByPostId(postId);
            if (exists is null)
                throw new NotFoundException($"Post {postId} not found");
        }
    }

}