using System.Security.Claims;
using GameDataServer.Auth;
using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Models;
using GameDataServer.Services;

namespace GameDataServer.Endpoints;

// 서버의 "화면" 역할: 주소(URL)를 Service에 연결하고, 결과를 HTTP 응답(200, 404 ...)으로 바꾼다
// 읽기(GET)는 누구나, 쓰기(POST·PUT·DELETE)는 로그인 토큰이 있어야 한다
public static class MonsterEndpoints
{
    private const int DefaultSlowMs = 3000;      // /monsters/slow 기본 대기 시간
    private const int DefaultBigCount = 20000;   // /monsters/big 기본 마릿수

    public static void MapMonsterEndpoints(this WebApplication app)
    {
        // 전체 조회
        app.MapGet("/monsters", async (IMonsterService service) =>
        {
            return Results.Ok(new { monsters = await service.GetAllAsync() });
        });

        // 테스트용 1: 일부러 느린 서버. ?ms=8000처럼 바꿀 수 있다
        app.MapGet("/monsters/slow", async (IMonsterService service, int ms = DefaultSlowMs) =>
        {
            return Results.Ok(new { monsters = await service.GetSlowAsync(ms) });
        });

        // 테스트용 2: 일부러 큰 데이터. ?count=500처럼 바꿀 수 있다
        app.MapGet("/monsters/big", async (IMonsterService service, int count = DefaultBigCount) =>
        {
            return Results.Ok(new { monsters = await service.GetBigAsync(count) });
        });

        // 하나 조회 — 없으면 404
        app.MapGet("/monsters/{id:int}", async (int id, IMonsterService service) =>
        {
            Result<Monster> result = await service.GetByIdAsync(id);
            return result switch
            {
                Result<Monster>.Success s => Results.Ok(s.Data),
                Result<Monster>.Failure f => f.Error.ToHttpResult(id),
                _ => Results.StatusCode(500),
            };
        });

        // 추가 — POST는 "받아라". 성공하면 201 Created. 누가 추가했는지는 토큰에서 꺼낸다
        app.MapPost("/monsters", async (MonsterDto dto, ClaimsPrincipal user, IMonsterService service) =>
        {
            Result<Monster> result = await service.AddAsync(dto);
            if (result is Result<Monster>.Success added)
            {
                app.Logger.LogInformation("몬스터 추가: {Name} (추가한 사람: {Player})", added.Data.Name, user.GetPlayerName());
            }

            return result switch
            {
                Result<Monster>.Success s => Results.Created($"/monsters/{s.Data.Id}", s.Data),
                Result<Monster>.Failure f => f.Error.ToHttpResult(),
                _ => Results.StatusCode(500),
            };
        }).RequireAuthorization();   // ← 자물쇠. 토큰이 없거나 틀리면 401

        // 고치기 — PUT은 "이걸로 바꿔라"
        app.MapPut("/monsters/{id:int}", async (int id, MonsterDto dto, IMonsterService service) =>
        {
            Result<Monster> result = await service.UpdateAsync(id, dto);
            return result switch
            {
                Result<Monster>.Success s => Results.Ok(s.Data),
                Result<Monster>.Failure f => f.Error.ToHttpResult(id),
                _ => Results.StatusCode(500),
            };
        }).RequireAuthorization();

        // 삭제 — DELETE는 "지워라". 성공하면 204 No Content
        app.MapDelete("/monsters/{id:int}", async (int id, IMonsterService service) =>
        {
            Result<int> result = await service.DeleteAsync(id);
            return result switch
            {
                Result<int>.Success => Results.NoContent(),
                Result<int>.Failure f => f.Error.ToHttpResult(id),
                _ => Results.StatusCode(500),
            };
        }).RequireAuthorization();
    }
}
