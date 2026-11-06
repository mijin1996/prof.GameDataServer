using System.Security.Claims;
using GameDataServer.Dtos;
using Microsoft.IdentityModel.JsonWebTokens;   // JWT 만들기
using Microsoft.IdentityModel.Tokens;          // 서명 도장

namespace GameDataServer.Auth;

// 플레이어 이름을 넣고 서명 키로 도장을 찍어 JWT를 만든다
public class JwtTokenService : ITokenService
{
    private const string TokenType = "Bearer";

    private readonly JwtSettings _settings;
    private readonly JsonWebTokenHandler _handler = new JsonWebTokenHandler();

    public JwtTokenService(JwtSettings settings)
    {
        _settings = settings;
    }

    public TokenResponse Create(string player)
    {
        string token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,                                                           // iss
            Audience = _settings.Audience,                                                       // aud
            Subject = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, player)]),      // sub: 누구의 토큰인가
            Expires = DateTime.UtcNow.AddMinutes(_settings.ExpiresMinutes),                      // exp
            SigningCredentials = new SigningCredentials(_settings.SigningKey, SecurityAlgorithms.HmacSha256),   // HS256 도장
        });

        return new TokenResponse(token, TokenType, _settings.ExpiresMinutes * 60);
    }
}
