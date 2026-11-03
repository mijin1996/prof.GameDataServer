using GameDataServer.Core;
using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;
using GameDataServer.Data.Repositories;

namespace GameDataServer.Services;

// 입력 검증과 게임 규칙을 맡는다. DB도, HTTP 주소도 모른다
public class MonsterService
{
    private readonly IMonsterRepository _repository;

    public MonsterService(IMonsterRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Monster>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // 테스트용 1: 일부러 ms 밀리초 늦게 대답한다 (Unity가 기다리는 동안 멈추는지 확인용)
    public async Task<List<Monster>> GetSlowAsync(int ms)
    {
        await Task.Delay(ms);
        return await _repository.GetAllAsync();
    }

    // 테스트용 2: 저장된 몬스터를 돌려 가며 복제해서 count마리짜리 큰 목록을 만든다
    public async Task<List<Monster>> GetBigAsync(int count)
    {
        List<Monster> monsters = await _repository.GetAllAsync();
        var big = new List<Monster>(count);
        if (monsters.Count == 0)
        {
            return big;   // 복제할 원본이 없으면 빈 목록 (0으로 나누기 방지)
        }

        for (int i = 1; i <= count; i++)
        {
            Monster src = monsters[(i - 1) % monsters.Count];
            big.Add(src with { Id = i, Name = $"{src.Name} #{i}" });
        }
        return big;
    }

    public Task<Result<Monster>> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public async Task<Result<Monster>> AddAsync(MonsterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))   // 규칙: 이름은 비면 안 된다
        {
            return new Result<Monster>.Failure(AppError.InvalidName);
        }

        Monster created = await _repository.AddAsync(dto.ToModel());
        return new Result<Monster>.Success(created);
    }

    public async Task<Result<Monster>> UpdateAsync(int id, MonsterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return new Result<Monster>.Failure(AppError.InvalidName);
        }

        return await _repository.UpdateAsync(id, dto.ToModel());
    }

    public Task<Result<int>> DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }
}
