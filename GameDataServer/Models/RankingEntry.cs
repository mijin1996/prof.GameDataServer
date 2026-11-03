namespace GameDataServer.Models;

// 랭킹 한 줄: 플레이어 이름(Id)과 점수
public record RankingEntry(string Id, int Score);
