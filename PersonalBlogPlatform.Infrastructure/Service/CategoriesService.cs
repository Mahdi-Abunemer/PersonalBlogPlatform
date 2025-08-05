using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
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
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoriesService(ICategoriesRepository categoryRepository, IMapper mapper , UserManager<ApplicationUser> userManager) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<CategoryResponse> AddCategory(CategoryAddRequest categoryAddRequest)
        {
            if (categoryAddRequest == null)
                throw new ArgumentNullException("Category shouldn't be empty", nameof(categoryAddRequest));

            ValidationHelper.ModelValidation(categoryAddRequest);

            var category = _mapper.Map<Category>(categoryAddRequest);

            category.Id = Guid.NewGuid();

            category.Author = await _userManager.FindByIdAsync(categoryAddRequest.AuthorId.ToString())
                         ?? throw new NotFoundException($"User {categoryAddRequest.AuthorId} not found");
            
            await _categoryRepository.AddCategory(category);

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task DeleteCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new InvalidIDException($"Comment ID:{categoryId} cannot be empty");

            var category = await _categoryRepository.GetCategoryByCategoryId(categoryId);
            if (category == null)
                throw new NotFoundException($"Category: {categoryId} is not found");

            await _categoryRepository.DeleteCategory(category);
        }

        public async Task<CategoryResponse> GetCategoryByCategoryId(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new InvalidIDException($"Comment ID:{categoryId} cannot be empty");

            var category = await _categoryRepository.GetCategoryByCategoryId(categoryId);
            if (category == null)
                throw new NotFoundException($"Category: {categoryId} is not found");

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> UpdateCategory(CategoryUpdateRequest categoryUpdateRequest)
        {
            if (categoryUpdateRequest == null)
                throw new ArgumentNullException(nameof(categoryUpdateRequest));

            if (categoryUpdateRequest.Id == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty", nameof(categoryUpdateRequest.Id));

            ValidationHelper.ModelValidation(categoryUpdateRequest);

            var category = await _categoryRepository.GetCategoryByCategoryId(categoryUpdateRequest.Id);
            if (category == null)
                throw new ArgumentNullException(nameof(category), $"Category {categoryUpdateRequest.Id} not found");

            category.CategoryName = categoryUpdateRequest.CategoryName;
            category.Slug = categoryUpdateRequest.Slug;

            await _categoryRepository.UpdateCategory(category);

            return _mapper.Map<CategoryResponse>(category);
        }
    }
}
