using PersonalBlogPlatform.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface ICategoriesService
    {
        Task<CategoryResponse> GetCategoryByCategoryId(Guid categoryId);

        Task<CategoryResponse> AddCategory(CategoryAddRequest categoryAddRequest);

        Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest categoryUpdateRequest);

        Task DeleteCategory(Guid categoryId);
    }
}
