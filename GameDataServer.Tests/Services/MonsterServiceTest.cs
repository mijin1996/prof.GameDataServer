using GameDataServer.Core;
using GameDataServer.Data.Repositories;
using GameDataServer.Dtos;
using GameDataServer.Models;
using GameDataServer.Services;

namespace GameDataServer.Tests.Services;

// MonsterService의 규칙을 DB 없이 확인한다
public class MonsterServiceTest
{
    private readonly FakeMonsterRepository _repository = new FakeMonsterRepository();
    private readonly MonsterService _service;

    public MonsterServiceTest()
    {
        _service = new MonsterService(_repository);
    }

    [Fact]
    public async Task 이름이_비면_추가하지_않고_InvalidName()
    {
        // When
        Result<Monster> result = await _service.AddAsync(new MonsterDto { Name = " ", Hp = 10 });

        // Then
        Assert.Equal(new Result<Monster>.Failure(AppError.InvalidName), result);
        Assert.Empty(_repository.Monsters);
    }

    [Fact]
    public async Task 큰_목록은_원본을_돌려_가며_복제한다()
    {
        // Given
        _repository.Monsters.Add(new Monster(1, "슬라임", 30, 5, 1.5f));
        _repository.Monsters.Add(new Monster(2, "고블린", 50, 8, 2.0f));

        // When
        List<Monster> big = await _service.GetBigAsync(3);

        // Then
        Assert.Equal(3, big.Count);
        Assert.Equal(new Monster(3, "슬라임 #3", 30, 5, 1.5f), big[2]);
    }

    [Fact]
    public async Task 큰_목록에_음수를_주면_빈_목록()
    {
        // Given
        _repository.Monsters.Add(new Monster(1, "슬라임", 30, 5, 1.5f));

        // When
        List<Monster> big = await _service.GetBigAsync(-5);

        // Then
        Assert.Empty(big);
    }

    [Fact]
    public async Task 느린_조회에_음수를_줘도_예외가_나지_않는다()
    {
        // When
        List<Monster> list = await _service.GetSlowAsync(-100);

        // Then
        Assert.Empty(list);
    }

    // ---- 가짜 부품 ----

    private class FakeMonsterRepository : IMonsterRepository
    {
        public List<Monster> Monsters { get; } = new List<Monster>();

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task<List<Monster>> GetAllAsync()
        {
            return Task.FromResult(Monsters.ToList());
        }

        public Task<Result<Monster>> GetByIdAsync(int id)
        {
            Monster? found = Monsters.Find(m => m.Id == id);
            Result<Monster> result = found == null
                ? new Result<Monster>.Failure(AppError.NotFound)
                : new Result<Monster>.Success(found);
            return Task.FromResult(result);
        }

        public Task<Monster> AddAsync(Monster monster)
        {
            Monster created = monster with { Id = Monsters.Count + 1 };
            Monsters.Add(created);
            return Task.FromResult(created);
        }

        public Task<Result<Monster>> UpdateAsync(int id, Monster monster)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
