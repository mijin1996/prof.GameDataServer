using GameDataServer.Core;
using GameDataServer.Data.DataSources;
using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;

namespace GameDataServer.Data.Repositories;

// DataSource를 숨기고, DTO를 Model로 바꿔서 돌려준다. "없음"은 Result로 알려 준다
public class MonsterRepository : IMonsterRepository
{
    private readonly IMonsterDataSource _dataSource;
    private readonly IMonsterSeedDataSource _seedDataSource;

    public MonsterRepository(IMonsterDataSource dataSource, IMonsterSeedDataSource seedDataSource)   // 생성자 주입
    {
        _dataSource = dataSource;
        _seedDataSource = seedDataSource;
    }

    public async Task InitializeAsync()
    {
        await _dataSource.CreateTableAsync();

        // 테이블이 비어 있을 때만 monsters.json을 넣는다. 그래서 재시작해도 중복되지 않는다
        if (await _dataSource.CountAsync() > 0)
        {
            return;
        }

        List<MonsterDto> seed = await _seedDataSource.LoadAsync();
        foreach (MonsterDto dto in seed)
        {
            await _dataSource.InsertAsync(dto);
        }
    }

    public async Task<List<Monster>> GetAllAsync()
    {
        List<MonsterDto> rows = await _dataSource.GetAllAsync();
        return rows.Select(dto => dto.ToModel()).ToList();
    }

    public async Task<Result<Monster>> GetByIdAsync(int id)
    {
        MonsterDto? dto = await _dataSource.GetByIdAsync(id);
        if (dto == null)
        {
            return new Result<Monster>.Failure(AppError.NotFound);
        }
        return new Result<Monster>.Success(dto.ToModel());
    }

    public async Task<Monster> AddAsync(Monster monster)
    {
        int newId = await _dataSource.InsertAsync(monster.ToDto());
        return monster with { Id = newId };   // DB가 붙여 준 id로 바꾼 복사본
    }

    public async Task<Result<Monster>> UpdateAsync(int id, Monster monster)
    {
        int changed = await _dataSource.UpdateAsync(id, monster.ToDto());
        if (changed == 0)
        {
            return new Result<Monster>.Failure(AppError.NotFound);
        }
        return new Result<Monster>.Success(monster with { Id = id });   // 본문의 id 대신 주소의 id
    }

    public async Task<Result<int>> DeleteAsync(int id)
    {
        int deleted = await _dataSource.DeleteAsync(id);
        if (deleted == 0)
        {
            return new Result<int>.Failure(AppError.NotFound);
        }
        return new Result<int>.Success(id);
    }
}
