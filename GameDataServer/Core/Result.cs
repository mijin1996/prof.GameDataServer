namespace GameDataServer.Core;

// 성공이면 Success(데이터), 실패면 Failure(실패 종류)를 돌려준다
public abstract record Result<T>
{
    public sealed record Success(T Data) : Result<T>;

    public sealed record Failure(AppError Error) : Result<T>;
}
