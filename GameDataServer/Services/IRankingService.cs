using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Models;

namespace GameDataServer.Services;

public interface IRankingService
{
    Task<Result<RankingEntry>> SaveAsync(RankingDto dto);
    Task<List<RankingEntry>> GetTopAsync();
}
