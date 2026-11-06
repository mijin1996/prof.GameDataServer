using GameDataServer.Core;
using GameDataServer.Data.DataSources;
using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

// DataSource를 숨기고, DTO를 Model로 바꿔서 돌려준다. "없음"은 Result로 알려 준다
public class PlayerRepository : IPlayerRepository
{
    private readonly IPlayerDataSource _dataSource;

    public PlayerRepository(IPlayerDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task InitializeAsync()
    {
        await _dataSource.CreateTableAsync();
    }

    public async Task<Result<Player>> GetByNameAsync(string name)
    {
        PlayerDto? dto = await _dataSource.GetByNameAsync(name);
        if (dto == null)
        {
            return new Result<Player>.Failure(AppError.NotFound);
        }
        return new Result<Player>.Success(dto.ToModel());
    }

    public async Task AddAsync(Player player)
    {
        await _dataSource.InsertAsync(player.ToDto());
    }
}
