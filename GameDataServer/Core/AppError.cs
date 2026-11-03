namespace GameDataServer.Core;

// 실패 종류를 이름으로 구분한다
public enum AppError
{
    NotFound,      // 그 id의 데이터가 없음 (데이터 실패 → Repository가 만든다)
    InvalidName,   // 이름이 비어 있음 (규칙 실패 → Service가 만든다)
}
