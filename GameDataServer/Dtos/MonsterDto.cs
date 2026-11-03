namespace GameDataServer.Dtos;

// 밖(요청 JSON, monsters.json, DB 한 줄)에서 들어온 날것의 데이터를 그대로 받는 그릇
// 값이 빠져 있을 수 있으니 전부 nullable(?)로 둔다
public class MonsterDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Hp { get; set; }
    public int? Attack { get; set; }
    public float? Speed { get; set; }
}
