using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Service;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;
using PersonalBlogPlatform.ServiceTests.CustomFixtureConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.ServiceTests
{
    public class RefreshTokenUseCaseTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IProfileService> _profileServiceMock;
        private readonly RefreshTokenUseCase _refreshTokenUseCase;

        public RefreshTokenUseCaseTests()
        {
            _fixture = AutoFixtureConfigurator.Create();

            _tokenServiceMock = new Mock<ITokenService>();
            _profileServiceMock = new Mock<IProfileService>();

            _refreshTokenUseCase = new RefreshTokenUseCase(_profileServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact]
        public async Task RenewAccessTokenAsync_WithValidRefreshToken_ReturnsNewToken()
        {
            var user = _fixture.Create<ApplicationUser>();

            var refreshToken = "refreshToken.";

            var newToken = _fixture.Create<TokenResponse>();

            _profileServiceMock.Setup(temp => temp.GetUserByRefreshToken(refreshToken)).ReturnsAsync(user);

            _profileServiceMock.Setup(temp => temp.ValidateRefreshToken(user, refreshToken)).Returns(true);

            _tokenServiceMock.Setup(temp => temp.CreateJwtToken(user))
               .ReturnsAsync(newToken);

            _profileServiceMock.Setup(p => p.StoreRefreshToken(user, newToken))
                .Returns(Task.CompletedTask);

            var result = await _refreshTokenUseCase.RenewAccessTokenAsync(refreshToken);

            result.Should().BeEquivalentTo(newToken);
        }

        [Fact]
        public async Task RenewAccessTokenAsync_ThrowsArgumentNullException_WhenRefreshTokenIsNull()
        {

            Func<Task> action = async () => await _refreshTokenUseCase.RenewAccessTokenAsync(null);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RenewAccessTokenAsync_ThrowsArgumentNullException_WhenRefreshTokenIsEmpty()
        {

            Func<Task> action = async () => await _refreshTokenUseCase.RenewAccessTokenAsync(string.Empty);

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task RenewAccessTokenAsync_ThrowsUnauthorizedAccessException_WhenUserNotFoundByRefreshToken()
        {
            var refreshToken = "refreshToken.";

            _profileServiceMock.Setup(temp => temp.GetUserByRefreshToken(refreshToken))
                               .ReturnsAsync(null as ApplicationUser);

            Func<Task> action = async () => await _refreshTokenUseCase.RenewAccessTokenAsync(refreshToken);

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task RenewAccessTokenAsync_ThrowsUnauthorizedAccessException_WhenRefreshTokenIsInvalid()
        {
            var user = _fixture.Create<ApplicationUser>();
            var refreshToken = "InvalidToken";

            _profileServiceMock.Setup(temp => temp.GetUserByRefreshToken(refreshToken))
                               .ReturnsAsync(user);

            _profileServiceMock.Setup(temp => temp.ValidateRefreshToken(user, refreshToken)).
                Returns(false);

            Func<Task> action = async () => await _refreshTokenUseCase.RenewAccessTokenAsync(refreshToken);

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
