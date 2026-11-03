using GameDataServer.Dtos;
using GameDataServer.Mappers;
using GameDataServer.Models;

namespace GameDataServer.Tests.Mappers;

// MonsterMapper가 DTO ↔ Model을 제대로 바꾸는지 확인한다
public class MonsterMapperTest
{
    [Fact]
    public void 값이_모두_있으면_그대로_Model이_된다()
    {
        // Given
        var dto = new MonsterDto { Id = 1, Name = "슬라임", Hp = 30, Attack = 5, Speed = 1.5f };

        // When
        Monster monster = dto.ToModel();

        // Then
        Assert.Equal(new Monster(1, "슬라임", 30, 5, 1.5f), monster);
    }

    [Fact]
    public void 빠진_값은_기본값으로_채운다()
    {
        // Given
        var dto = new MonsterDto();

        // When
        Monster monster = dto.ToModel();

        // Then
        Assert.Equal(new Monster(0, "", 0, 0, 0f), monster);
    }

    [Fact]
    public void Model을_DTO로_바꾸면_값이_같다()
    {
        // Given
        var monster = new Monster(2, "고블린", 50, 8, 2.0f);

        // When
        MonsterDto dto = monster.ToDto();

        // Then
        Assert.Equal(2, dto.Id);
        Assert.Equal("고블린", dto.Name);
        Assert.Equal(50, dto.Hp);
        Assert.Equal(8, dto.Attack);
        Assert.Equal(2.0f, dto.Speed);
    }
}
