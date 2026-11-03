using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Models;
using GameDataServer.Services;

namespace GameDataServer.Endpoints;

public static class RankingEndpoints
{
    public static void MapRankingEndpoints(this WebApplication app)
    {
        // 랭킹 저장 — 같은 이름이면 더 높은 점수만 남는다
        app.MapPost("/ranking", async (RankingDto dto, RankingService service) =>
        {
            Result<RankingEntry> result = await service.SaveAsync(dto);
            return result switch
            {
                Result<RankingEntry>.Success s => Results.Ok(s.Data),
                Result<RankingEntry>.Failure => Results.BadRequest(new { error = "이름은 비어 있으면 안 됩니다." }),
                _ => Results.StatusCode(500),
            };
        });

        // 랭킹 top 10 조회
        app.MapGet("/ranking/top", async (RankingService service) =>
        {
            return Results.Ok(new { ranking = await service.GetTopAsync() });
        });
    }
}
