using GameDataServer.Dtos;
using GameDataServer.Models;

namespace GameDataServer.Mappers;

// 랭킹 DTO ↔ Model 변환
public static class RankingMapper
{
    public static RankingEntry ToModel(this RankingDto dto)
    {
        return new RankingEntry(dto.Id ?? "", dto.Score ?? 0);
    }

    public static RankingDto ToDto(this RankingEntry entry)
    {
        return new RankingDto
        {
            Id = entry.Id,
            Score = entry.Score,
        };
    }
}
