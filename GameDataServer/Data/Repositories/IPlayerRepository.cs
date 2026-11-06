using GameDataServer.Core;
using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

// 바깥(AuthService)은 플레이어가 어디에 저장되는지 모르고 Model만 받는다
public interface IPlayerRepository
{
    Task InitializeAsync();                           // 테이블 준비
    Task<Result<Player>> GetByNameAsync(string name); // 없으면 Failure(NotFound)
    Task AddAsync(Player player);
}
