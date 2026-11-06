using GameDataServer.Dtos;

namespace GameDataServer.Data.DataSources;

// 플레이어 표(players)와 직접 통신하는 약속
public interface IPlayerDataSource
{
    Task CreateTableAsync();                        // 테이블이 없으면 만들기
    Task<PlayerDto?> GetByNameAsync(string name);   // 이름으로 찾기. 없으면 null
    Task InsertAsync(PlayerDto dto);                // 새 플레이어 저장
}
