using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GameDataServer.Auth;

// 토큰 설정을 한곳에 모은다. 발급(JwtTokenService)과 검사(AuthExtensions)가 같은 값을 쓴다
public record JwtSettings(string Key, string Issuer, string Audience, int ExpiresMinutes)
{
    // 내 PC용 키. 배포(Render)에서는 환경 변수 JWT_KEY로 넣는다
    private const string LocalDevKey = "local-dev-only-key-0123456789-abcdefghijklmnopqrstuvwxyz";
    private const int MinKeyBytes = 32;   // HS256은 32바이트(256비트) 이상이어야 한다

    public static JwtSettings FromEnvironment()
    {
        string key = Environment.GetEnvironmentVariable("JWT_KEY") ?? LocalDevKey;
        if (Encoding.UTF8.GetByteCount(key) < MinKeyBytes)
        {
            throw new InvalidOperationException($"JWT_KEY는 {MinKeyBytes}바이트 이상이어야 합니다.");
        }

        return new JwtSettings(
            Key: key,
            Issuer: "GameDataServer",   // 누가 발급했나 (iss)
            Audience: "MonsterDex",     // 누구에게 쓰라고 발급했나 (aud)
            ExpiresMinutes: 30);        // 30분 뒤 만료 (exp)
    }

    public SymmetricSecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
