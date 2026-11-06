namespace GameDataServer.Models;

// 플레이어 한 명: 이름과 비밀번호 해시. 비밀번호 원문은 어디에도 저장하지 않는다
public record Player(string Name, string PasswordHash);
