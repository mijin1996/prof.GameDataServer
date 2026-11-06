using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;
using GameDataServer.Data.Repositories;

namespace GameDataServer.Services;

// 랭킹 규칙: 이름은 비면 안 되고, 같은 이름은 최고 점수만 남는다, 상위 10명만 보여 준다
public class RankingService : IRankingService
{
    private const int TopCount = 10;

    private readonly IRankingRepository _repository;

    public RankingService(IRankingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<RankingEntry>> SaveAsync(RankingDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            return new Result<RankingEntry>.Failure(AppError.InvalidName);
        }

        RankingEntry entry = dto.ToModel();
        await _repository.SaveBestScoreAsync(entry);
        return new Result<RankingEntry>.Success(entry);
    }

    public Task<List<RankingEntry>> GetTopAsync()
    {
        return _repository.GetTopAsync(TopCount);
    }
}
