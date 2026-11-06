using GameDataServer.Auth;
using GameDataServer.Core;
using GameDataServer.Data.Repositories;
using GameDataServer.Dtos;
using GameDataServer.Models;
using Microsoft.AspNetCore.Identity;   // 비밀번호 해시

namespace GameDataServer.Services;

// 로그인 규칙을 맡는다. DB도, HTTP도, 토큰을 어떻게 만드는지도 모른다
// 수업용 간소화: 처음 보는 이름이면 그 비밀번호로 가입시키고 바로 토큰을 준다
public class AuthService : IAuthService
{
    private readonly IPlayerRepository _players;
    private readonly IPasswordHasher<string> _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IPlayerRepository players, IPasswordHasher<string> hasher, ITokenService tokens)
    {
        _players = players;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Player) || string.IsNullOrEmpty(dto.Password))   // 규칙: 둘 다 있어야 한다
        {
            return new Result<TokenResponse>.Failure(AppError.InvalidLogin);
        }

        string name = dto.Player.Trim();
        Result<Player> found = await _players.GetByNameAsync(name);

        if (found is Result<Player>.Success existing)
        {
            PasswordVerificationResult check = _hasher.VerifyHashedPassword(name, existing.Data.PasswordHash, dto.Password);
            if (check == PasswordVerificationResult.Failed)
            {
                return new Result<TokenResponse>.Failure(AppError.WrongPassword);
            }
        }
        else
        {
            // 처음 보는 이름 → 가입. 비밀번호는 해시로만 저장한다
            await _players.AddAsync(new Player(name, _hasher.HashPassword(name, dto.Password)));
        }

        return new Result<TokenResponse>.Success(_tokens.Create(name));
    }
}
