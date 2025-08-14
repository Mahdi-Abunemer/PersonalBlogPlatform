using AutoFixture;
using AutoMapper;
using Azure.Core;
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
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.ServiceTests
{
    public class CategoriesServiceTest
    {
        private readonly IFixture _fixture;
        private readonly Mock<ICategoriesRepository> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ICategoriesService _categoriesService;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public CategoriesServiceTest()
        {
            _fixture = AutoFixtureConfigurator.Create();

            _categoryRepositoryMock = new Mock<ICategoriesRepository>();

            _mapperMock = new Mock<IMapper>();
            _mapperMock
                .Setup(m => m.Map<Category>(It.IsAny<CategoryAddRequest>()))
                .Returns((CategoryAddRequest req) => new Category
                {
                    CategoryName = req.CategoryName,
                    Slug = req.Slug
                });

            _mapperMock
                .Setup(m => m.Map<CategoryResponse>(It.IsAny<Category>()))
                .Returns((Category src) => new CategoryResponse
                {
                    Id = src.Id,
                    CategoryName = src.CategoryName,
                    Slug = src.Slug
                });

            _userManagerMock = CreateUserManagerMock<ApplicationUser>();
            _categoriesService = new CategoriesService(_categoryRepositoryMock.Object , _mapperMock.Object ,
                                       _userManagerMock.Object);
        }

        private static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        #region Add

        [Fact]
        public async Task AddCategory_WhenRequestIsValid_ReturnsCreatedCategory()
        {
            // Arrange
            var author = _fixture.Build<ApplicationUser>()
                .With(u => u.Id, Guid.NewGuid())
                .Create();

            var request = _fixture.Build<CategoryAddRequest>()
                .With(r => r.AuthorId, author.Id)
                .Create();
          
            _userManagerMock
                .Setup(u => u.FindByIdAsync(author.Id.ToString()))
                .ReturnsAsync(author);

            Category capturedCategory = null;

            _categoryRepositoryMock
                .Setup(r => r.AddCategory(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .Returns(() => Task.FromResult(capturedCategory)); 

            // Act
            var result = await _categoriesService.AddCategory(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.CategoryName.Should().Be(capturedCategory.CategoryName);
            result.Slug.Should().Be(capturedCategory.Slug);

            // Verify interactions
            _userManagerMock.Verify(u => u.FindByIdAsync(author.Id.ToString()), Times.Once);
            _categoryRepositoryMock.Verify(r => r.AddCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task AddCategory_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () =>  await _categoriesService.AddCategory(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddCategory_WhenAuthorNotFound_ThrowsNotFoundException()
        {
            var author = _fixture.Build<ApplicationUser>()
                .With(u => u.Id, Guid.NewGuid())
                .Create();

            var request = _fixture.Build<CategoryAddRequest>()
                .With(r => r.AuthorId, author.Id)
                .Create();

            _userManagerMock
                .Setup(u => u.FindByIdAsync(author.Id.ToString()))
                .ReturnsAsync(null as ApplicationUser);

            Category capturedCategory = null;

            _categoryRepositoryMock
                .Setup(r => r.AddCategory(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .Returns(() => Task.FromResult(capturedCategory));

            Func<Task> act = async () => await _categoriesService.AddCategory(request);

            await act.Should().ThrowAsync<NotFoundException>();

        }
        #endregion

        #region Delete
        [Fact]
        public async Task DeleteCategory_WhenCategoryExists_DeletesCategory()
        {
            var categoryId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            _categoryRepositoryMock
                .Setup(temp => temp.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(temp => temp.DeleteCategory(category))
                .Returns(Task.CompletedTask);


            Func<Task> act = async () => await _categoriesService.DeleteCategory(categoryId);

            await act.Should().NotThrowAsync();

            _categoryRepositoryMock.Verify(repo => repo.DeleteCategory(category), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _categoriesService.DeleteCategory(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task DeleteCategory_WhenCategoryNotFound_ThrowsNotFoundException()
        {
            var categoryId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(temp => temp.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(null as Category);

            Func<Task> act = async () => await _categoriesService.DeleteCategory(categoryId);

            await act.Should().ThrowAsync<NotFoundException>();
        }
        #endregion

        #region Get
        [Fact]
        public async Task GetCategoryByCategoryId_WhenCategoryExists_ReturnsCategory()
        {
            var categoryId = Guid.NewGuid();

            var category = _fixture.Build<Category>()
                .With(c => c.Id, categoryId)
                .Create();

            _categoryRepositoryMock
                .Setup(temp => temp.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(category);

            Func<Task> act = async () => await _categoriesService.GetCategoryByCategoryId(categoryId);

            await act.Should().NotThrowAsync();

            _categoryRepositoryMock.Verify(repo => repo.GetCategoryByCategoryId(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetCategoryByCategoryId_WhenIdIsEmpty_ThrowsInvalidIDException()
        {
            Func<Task> act = async () => await _categoriesService.GetCategoryByCategoryId(Guid.Empty);

            await act.Should().ThrowAsync<InvalidIDException>();
        }

        [Fact]
        public async Task GetCategoryByCategoryId_WhenCategoryNotFound_ThrowsNotFoundException()
        {
            var categoryId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(temp => temp.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(null as Category);

            Func<Task> act = async () => await _categoriesService.GetCategoryByCategoryId(categoryId);

            await act.Should().ThrowAsync<NotFoundException>();
        }
        #endregion

        #region Update

        [Fact]
        public async Task UpdateCategory_WhenRequestIsValid_ReturnsUpdatedCategory()
        {
            var request = _fixture.Build<CategoryUpdateRequest>()
                .Create();

            var category = _fixture.Build<Category>()
                .With(i => i.Id , request.Id)
                .With(cn => cn.CategoryName , request.CategoryName)
                .With(c => c.Slug , request.Slug)
                .Create();

            _categoryRepositoryMock
                .Setup(repo => repo.GetCategoryByCategoryId(request.Id))
                .ReturnsAsync(category);

            _categoryRepositoryMock
                .Setup(temp => temp.UpdateCategory(category))
                .Returns(Task.CompletedTask);

            var result = await _categoriesService.UpdateCategory(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.CategoryName.Should().Be(request.CategoryName);
            result.Slug.Should().Be(request.Slug);

            _categoryRepositoryMock.Verify(r => r.UpdateCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_WhenRequestIsNull_ThrowsArgumentNullException()
        {
            Func<Task> act = async () => await _categoriesService.UpdateCategory(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task UpdateCategory_WhenIdIsEmpty_ThrowsArgumentException()
        {
            var request = _fixture.Build<CategoryUpdateRequest>()
                .With(i => i.Id , Guid.Empty)
                .Create();

            Func<Task> act = async () => await _categoriesService.UpdateCategory(request);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateCategory_WhenCategoryNotFound_ThrowsArgumentNullException()
        {
            var categoryId = Guid.NewGuid();

            var request = _fixture.Build<CategoryUpdateRequest>()
                .With(i => i.Id, categoryId)
                .Create();

            _categoryRepositoryMock
                .Setup(temp => temp.GetCategoryByCategoryId(categoryId))
                .ReturnsAsync(null as Category);

            Func<Task> act = async () => await _categoriesService.UpdateCategory(request);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
        #endregion
    }
}
