using System.Security.Claims;
using GameDataServer.Auth;
using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Services;

namespace GameDataServer.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // 로그인 — 이름과 비밀번호를 확인하고 토큰을 발급한다
        app.MapPost("/auth/login", async (LoginDto dto, IAuthService service) =>
        {
            Result<TokenResponse> result = await service.LoginAsync(dto);
            return result switch
            {
                Result<TokenResponse>.Success s => Results.Ok(s.Data),
                Result<TokenResponse>.Failure f => f.Error.ToHttpResult(),
                _ => Results.StatusCode(500),
            };
        });

        // 내 정보 — 토큰만 보내면 서버가 내가 누구인지 알려 준다
        app.MapGet("/auth/me", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                player = user.GetPlayerName(),
                expiresAt = user.GetExpiresAt(),
            });
        }).RequireAuthorization();
    }
}
