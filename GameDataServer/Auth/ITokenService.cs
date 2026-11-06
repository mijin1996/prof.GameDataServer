using GameDataServer.Dtos;

namespace GameDataServer.Auth;

// 토큰을 만드는 약속. AuthService는 JWT인지 다른 방식인지 모른다 (테스트에서는 가짜로 바꿔 끼운다)
public interface ITokenService
{
    TokenResponse Create(string player);
}
