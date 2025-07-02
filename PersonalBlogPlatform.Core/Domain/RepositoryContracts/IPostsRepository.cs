using PersonalBlogPlatform.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Post entity
    /// </summary>
    public interface IPostsRepository
    {
        /// <summary>
        /// Adds a post objetc to the database  
        /// </summary>
        /// <param name="post">Post object to add</param>
        /// <returns>Returns the post object added</returns>
       Task<Post> AddPost(Post post);   

        /// <summary>
        /// Update a post object
        /// </summary>
        /// <param name="post">Post object to update</param>
        /// <returns>Returns the post updated object</returns>
        Task UpdatePost(Post post);

        /// <summary>
        /// Delete post object from the database based on post id 
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
        Task DeletePost(Post post);

        /// <summary>
        /// Return All posts from the database
        /// </summary>
        /// <returns>Return posts list from the table</returns>
        Task<List<Post>> GetAllPosts();

        /// <summary>
        /// Get post object based on post id 
        /// </summary>
        /// <param name="postId"></param>
        /// <returns>Returns post object</returns>
        Task<Post?> GetPostByPostId(Guid postId);

        /// <summary>
        /// Get Latest Posts from the database
        /// </summary>
        /// <returns>Returns the latest posts from the table</returns>
        Task<List<Post>> GetLatestPosts(int count=5);

        /// <summary>
        /// Get filtered posts based on category id from the database 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>Returns the post list from the table</returns>
        Task<List<Post>> GetFilteredPosts (Guid categoryId);
    }
}
