using GameDataServer.Auth;
using GameDataServer.Core;
using GameDataServer.Data.Repositories;
using GameDataServer.Dtos;
using GameDataServer.Models;
using GameDataServer.Services;
using Microsoft.AspNetCore.Identity;

namespace GameDataServer.Tests.Services;

// AuthService의 로그인 규칙을 DB 없이 확인한다
// Repository와 TokenService를 가짜로 바꿔 끼울 수 있는 건 인터페이스로 받기 때문이다 (의존성 역전)
public class AuthServiceTest
{
    private readonly FakePlayerRepository _players = new FakePlayerRepository();
    private readonly AuthService _service;

    public AuthServiceTest()
    {
        _service = new AuthService(_players, new PasswordHasher<string>(), new FakeTokenService());
    }

    [Theory]
    [InlineData(null, "1234")]
    [InlineData("  ", "1234")]
    [InlineData("kim", "")]
    public async Task 이름이나_비밀번호가_비면_InvalidLogin(string? player, string? password)
    {
        // When
        Result<TokenResponse> result = await _service.LoginAsync(new LoginDto { Player = player, Password = password });

        // Then
        Assert.Equal(new Result<TokenResponse>.Failure(AppError.InvalidLogin), result);
        Assert.Empty(_players.Saved);
    }

    [Fact]
    public async Task 처음_보는_이름이면_가입하고_토큰을_준다()
    {
        // When
        Result<TokenResponse> result = await _service.LoginAsync(new LoginDto { Player = " kim ", Password = "1234" });

        // Then
        var success = Assert.IsType<Result<TokenResponse>.Success>(result);
        Assert.Equal("token-for-kim", success.Data.AccessToken);   // 앞뒤 공백은 잘라 낸다
        Assert.Single(_players.Saved);
        Assert.Equal("kim", _players.Saved[0].Name);
    }

    [Fact]
    public async Task 비밀번호는_원문이_아니라_해시로_저장된다()
    {
        // When
        await _service.LoginAsync(new LoginDto { Player = "kim", Password = "1234" });

        // Then
        Assert.NotEqual("1234", _players.Saved[0].PasswordHash);
    }

    [Fact]
    public async Task 비밀번호가_맞으면_토큰을_준다()
    {
        // Given
        await _service.LoginAsync(new LoginDto { Player = "kim", Password = "1234" });

        // When
        Result<TokenResponse> result = await _service.LoginAsync(new LoginDto { Player = "kim", Password = "1234" });

        // Then
        Assert.IsType<Result<TokenResponse>.Success>(result);
        Assert.Single(_players.Saved);   // 다시 가입하지 않는다
    }

    [Fact]
    public async Task 비밀번호가_틀리면_WrongPassword()
    {
        // Given
        await _service.LoginAsync(new LoginDto { Player = "kim", Password = "1234" });

        // When
        Result<TokenResponse> result = await _service.LoginAsync(new LoginDto { Player = "kim", Password = "9999" });

        // Then
        Assert.Equal(new Result<TokenResponse>.Failure(AppError.WrongPassword), result);
    }

    // ---- 가짜 부품 ----

    private class FakePlayerRepository : IPlayerRepository
    {
        public List<Player> Saved { get; } = new List<Player>();

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<Result<Player>> GetByNameAsync(string name)
        {
            Player? found = Saved.Find(p => p.Name == name);
            Result<Player> result = found == null
                ? new Result<Player>.Failure(AppError.NotFound)
                : new Result<Player>.Success(found);
            return Task.FromResult(result);
        }

        public Task AddAsync(Player player)
        {
            Saved.Add(player);
            return Task.CompletedTask;
        }
    }

    private class FakeTokenService : ITokenService
    {
        public TokenResponse Create(string player)
        {
            return new TokenResponse($"token-for-{player}", "Bearer", 1800);
        }
    }
}
