namespace GameDataServer.Dtos;

// 랭킹 요청 JSON과 DB 한 줄을 그대로 받는 그릇
public class RankingDto
{
    public string? Id { get; set; }
    public int? Score { get; set; }
}
