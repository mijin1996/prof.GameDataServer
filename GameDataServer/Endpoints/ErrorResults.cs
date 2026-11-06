using GameDataServer.Core;

namespace GameDataServer.Endpoints;

// 실패 종류(AppError)를 HTTP 응답으로 바꾸는 곳은 여기 한 곳뿐이다
// 새 AppError를 추가해도 각 Endpoints 파일은 고치지 않고 이 switch에 한 줄만 더한다
public static class ErrorResults
{
    public static IResult ToHttpResult(this AppError error, int id = 0)
    {
        return error switch
        {
            AppError.NotFound => Results.NotFound(new { error = $"id {id} 몬스터가 없습니다." }),
            AppError.InvalidName => Results.BadRequest(new { error = "이름은 비어 있으면 안 됩니다." }),
            AppError.InvalidLogin => Results.BadRequest(new { error = "player와 password는 비어 있으면 안 됩니다." }),
            AppError.WrongPassword => Results.Json(new { error = "비밀번호가 틀렸습니다." }, statusCode: StatusCodes.Status401Unauthorized),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}
