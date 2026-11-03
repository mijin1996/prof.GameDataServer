using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

public interface IRankingRepository
{
    Task InitializeAsync();
    Task SaveBestScoreAsync(RankingEntry entry);
    Task<List<RankingEntry>> GetTopAsync(int limit);
}
