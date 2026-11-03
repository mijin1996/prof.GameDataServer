using GameDataServer.Dtos;

namespace GameDataServer.Data.DataSources;

// 랭킹 DB와 직접 통신하는 약속
public interface IRankingDataSource
{
    Task CreateTableAsync();                       // 테이블이 없으면 만들기
    Task UpsertBestScoreAsync(RankingDto dto);     // 없으면 추가, 있으면 더 높은 점수만 남기기
    Task<List<RankingDto>> GetTopAsync(int limit); // 점수 높은 순으로 limit개
}
