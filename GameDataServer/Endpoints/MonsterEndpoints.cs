using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Models;
using GameDataServer.Services;

namespace GameDataServer.Endpoints;

// 서버의 "화면" 역할: 주소(URL)를 Service에 연결하고, 결과를 HTTP 응답(200, 404 ...)으로 바꾼다
public static class MonsterEndpoints
{
    private const int DefaultSlowMs = 3000;      // /monsters/slow 기본 대기 시간
    private const int DefaultBigCount = 20000;   // /monsters/big 기본 마릿수

    public static void MapMonsterEndpoints(this WebApplication app)
    {
        // 전체 조회
        app.MapGet("/monsters", async (MonsterService service) =>
        {
            return Results.Ok(new { monsters = await service.GetAllAsync() });
        });

        // 테스트용 1: 일부러 느린 서버. ?ms=8000처럼 바꿀 수 있다
        app.MapGet("/monsters/slow", async (MonsterService service, int ms = DefaultSlowMs) =>
        {
            return Results.Ok(new { monsters = await service.GetSlowAsync(ms) });
        });

        // 테스트용 2: 일부러 큰 데이터. ?count=500처럼 바꿀 수 있다
        app.MapGet("/monsters/big", async (MonsterService service, int count = DefaultBigCount) =>
        {
            return Results.Ok(new { monsters = await service.GetBigAsync(count) });
        });

        // 하나 조회 — 없으면 404
        app.MapGet("/monsters/{id:int}", async (int id, MonsterService service) =>
        {
            Result<Monster> result = await service.GetByIdAsync(id);
            return result switch
            {
                Result<Monster>.Success s => Results.Ok(s.Data),
                Result<Monster>.Failure f => ToErrorResult(f.Error, id),
                _ => Results.StatusCode(500),
            };
        });

        // 추가 — POST는 "받아라". 성공하면 201 Created
        app.MapPost("/monsters", async (MonsterDto dto, MonsterService service) =>
        {
            Result<Monster> result = await service.AddAsync(dto);
            return result switch
            {
                Result<Monster>.Success s => Results.Created($"/monsters/{s.Data.Id}", s.Data),
                Result<Monster>.Failure f => ToErrorResult(f.Error, 0),
                _ => Results.StatusCode(500),
            };
        });

        // 고치기 — PUT은 "이걸로 바꿔라"
        app.MapPut("/monsters/{id:int}", async (int id, MonsterDto dto, MonsterService service) =>
        {
            Result<Monster> result = await service.UpdateAsync(id, dto);
            return result switch
            {
                Result<Monster>.Success s => Results.Ok(s.Data),
                Result<Monster>.Failure f => ToErrorResult(f.Error, id),
                _ => Results.StatusCode(500),
            };
        });

        // 삭제 — DELETE는 "지워라". 성공하면 204 No Content
        app.MapDelete("/monsters/{id:int}", async (int id, MonsterService service) =>
        {
            Result<int> result = await service.DeleteAsync(id);
            return result switch
            {
                Result<int>.Success => Results.NoContent(),
                Result<int>.Failure f => ToErrorResult(f.Error, id),
                _ => Results.StatusCode(500),
            };
        });
    }

    // 실패 종류(AppError)를 HTTP 응답으로 바꾼다
    private static IResult ToErrorResult(AppError error, int id)
    {
        return error switch
        {
            AppError.NotFound => Results.NotFound(new { error = $"id {id} 몬스터가 없습니다." }),
            AppError.InvalidName => Results.BadRequest(new { error = "name은 비어 있으면 안 됩니다." }),
            _ => Results.StatusCode(500),
        };
    }
}
