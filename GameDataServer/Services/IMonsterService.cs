using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Models;

namespace GameDataServer.Services;

// Endpoints는 이 약속에만 기대고, 실제 MonsterService는 모른다 (의존성 역전)
public interface IMonsterService
{
    Task<List<Monster>> GetAllAsync();
    Task<List<Monster>> GetSlowAsync(int ms);
    Task<List<Monster>> GetBigAsync(int count);
    Task<Result<Monster>> GetByIdAsync(int id);
    Task<Result<Monster>> AddAsync(MonsterDto dto);
    Task<Result<Monster>> UpdateAsync(int id, MonsterDto dto);
    Task<Result<int>> DeleteAsync(int id);
}
