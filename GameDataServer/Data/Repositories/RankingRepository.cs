using GameDataServer.Data.DataSources;
using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

public class RankingRepository : IRankingRepository
{
    private readonly IRankingDataSource _dataSource;

    public RankingRepository(IRankingDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task InitializeAsync()
    {
        await _dataSource.CreateTableAsync();
    }

    public async Task SaveBestScoreAsync(RankingEntry entry)
    {
        await _dataSource.UpsertBestScoreAsync(entry.ToDto());
    }

    public async Task<List<RankingEntry>> GetTopAsync(int limit)
    {
        List<RankingDto> rows = await _dataSource.GetTopAsync(limit);
        return rows.Select(dto => dto.ToModel()).ToList();
    }
}
