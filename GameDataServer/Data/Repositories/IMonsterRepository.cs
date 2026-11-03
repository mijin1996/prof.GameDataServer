using GameDataServer.Core;
using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

// 바깥(Service)은 DB가 SQLite인지 파일인지 모르고 Model만 받는다
public interface IMonsterRepository
{
    Task InitializeAsync();                                   // 테이블 준비 + 비어 있으면 씨앗 데이터 넣기
    Task<List<Monster>> GetAllAsync();
    Task<Result<Monster>> GetByIdAsync(int id);
    Task<Monster> AddAsync(Monster monster);                  // 새 id가 붙은 몬스터를 돌려준다
    Task<Result<Monster>> UpdateAsync(int id, Monster monster);
    Task<Result<int>> DeleteAsync(int id);                    // 성공하면 지운 id
}
