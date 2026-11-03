using GameDataServer.Dtos;

namespace GameDataServer.Data.DataSources;

// 몬스터 DB와 직접 통신(CRUD)하는 약속
public interface IMonsterDataSource
{
    Task CreateTableAsync();                          // 테이블이 없으면 만들기
    Task<long> CountAsync();                          // 몇 마리 있는지
    Task<List<MonsterDto>> GetAllAsync();             // 전부 읽기
    Task<MonsterDto?> GetByIdAsync(int id);           // 하나 읽기. 없으면 null
    Task<int> InsertAsync(MonsterDto dto);            // 추가하고 새 id 돌려주기
    Task<int> UpdateAsync(int id, MonsterDto dto);    // 바뀐 줄 수 (없으면 0)
    Task<int> DeleteAsync(int id);                    // 지운 줄 수 (없으면 0)
}
