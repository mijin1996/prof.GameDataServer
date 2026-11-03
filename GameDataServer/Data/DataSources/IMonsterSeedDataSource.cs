using GameDataServer.Dtos;

namespace GameDataServer.Data.DataSources;

// 처음 DB가 비어 있을 때 넣을 초기 데이터(씨앗)를 읽는 약속
public interface IMonsterSeedDataSource
{
    Task<List<MonsterDto>> LoadAsync();
}
