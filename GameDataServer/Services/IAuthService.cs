using GameDataServer.Core;
using GameDataServer.Dtos;

namespace GameDataServer.Services;

public interface IAuthService
{
    Task<Result<TokenResponse>> LoginAsync(LoginDto dto);
}
