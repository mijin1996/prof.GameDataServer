namespace GameDataServer.Dtos;

// POST /auth/login 요청 JSON: { "player": "kim", "password": "1234" }
public class LoginDto
{
    public string? Player { get; set; }
    public string? Password { get; set; }
}
