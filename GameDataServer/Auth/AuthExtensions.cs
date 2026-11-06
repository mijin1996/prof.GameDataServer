using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;   // Bearer 토큰 검사기
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GameDataServer.Auth;

// Program.cs가 "조립"만 하도록 인증 등록을 여기로 모았다
public static class AuthExtensions
{
    // 들어온 토큰을 어떻게 검사할지 등록한다
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings settings)
    {
        services.AddSingleton(settings);
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;   // 토큰의 "sub"를 이름 그대로 쓴다
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,              // 내가 발급한 것인가
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,          // 이 게임용인가
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = settings.SigningKey,     // 서명이 맞는가
                    ValidateLifetime = true,                    // 만료되지 않았나
                    ClockSkew = TimeSpan.FromSeconds(30),       // 시계 오차 30초까지 봐준다
                };
            });

        services.AddAuthorization();
        return services;
    }

    // 검사를 통과한 토큰에서 플레이어 이름(sub)을 꺼낸다
    public static string? GetPlayerName(this ClaimsPrincipal user)
    {
        return user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }

    // 토큰 만료 시각(exp)을 꺼낸다
    public static DateTimeOffset? GetExpiresAt(this ClaimsPrincipal user)
    {
        string? exp = user.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        return long.TryParse(exp, out long seconds) ? DateTimeOffset.FromUnixTimeSeconds(seconds) : null;
    }
}
