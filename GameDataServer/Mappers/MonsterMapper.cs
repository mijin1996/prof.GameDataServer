using GameDataServer.Dtos;
using GameDataServer.Models;

namespace GameDataServer.Mappers;

// DTO ↔ Model 변환만 한다. 빠진 값은 기본값으로 채운다 (방어 로직)
public static class MonsterMapper
{
    public static Monster ToModel(this MonsterDto dto)
    {
        return new Monster(
            dto.Id ?? 0,
            dto.Name ?? "",
            dto.Hp ?? 0,
            dto.Attack ?? 0,
            dto.Speed ?? 0f);
    }

    public static MonsterDto ToDto(this Monster monster)
    {
        return new MonsterDto
        {
            Id = monster.Id,
            Name = monster.Name,
            Hp = monster.Hp,
            Attack = monster.Attack,
            Speed = monster.Speed,
        };
    }
}
