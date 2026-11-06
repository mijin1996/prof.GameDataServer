namespace GameDataServer.Core;

// 실패 종류를 이름으로 구분한다
// 새 실패를 추가하면 Endpoints/ErrorResults.cs 한 곳에만 HTTP 응답을 연결하면 된다
public enum AppError
{
    NotFound,        // 그 id의 데이터가 없음 (데이터 실패 → Repository가 만든다)
    InvalidName,     // 이름이 비어 있음 (규칙 실패 → Service가 만든다)
    InvalidLogin,    // player 또는 password가 비어 있음 (규칙 실패 → AuthService가 만든다)
    WrongPassword,   // 비밀번호가 틀림 (규칙 실패 → AuthService가 만든다)
}
