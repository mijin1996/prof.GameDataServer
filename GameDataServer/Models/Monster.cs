namespace GameDataServer.Models;

// 몬스터 데이터 모양만 정의한다. 기능 없음, null 없음, 한번 만들면 안 바뀜(record)
public record Monster(int Id, string Name, int Hp, int Attack, float Speed);
