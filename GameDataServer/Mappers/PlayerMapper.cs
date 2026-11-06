using GameDataServer.Dtos;
using GameDataServer.Models;

namespace GameDataServer.Mappers;

// 플레이어 DTO ↔ Model 변환
public static class PlayerMapper
{
    public static Player ToModel(this PlayerDto dto)
    {
        return new Player(dto.Name ?? "", dto.PasswordHash ?? "");
    }

    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto
        {
            Name = player.Name,
            PasswordHash = player.PasswordHash,
        };
    }
}
