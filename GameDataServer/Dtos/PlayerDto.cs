namespace GameDataServer.Dtos;

// players 테이블 한 줄을 그대로 받는 그릇
public class PlayerDto
{
    public string? Name { get; set; }
    public string? PasswordHash { get; set; }
}
