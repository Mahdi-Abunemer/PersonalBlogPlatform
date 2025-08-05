using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface IPostCategoryService
    {
        Task AddPostToCategoryAsync(Guid categoryId, Guid postId);

        Task RemovePostFromCategoryAsync(Guid categoryId, Guid postId);
    }
}
