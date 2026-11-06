using Microsoft.Data.Sqlite;

namespace GameDataServer.Data.DataSources;

// DB 파일 하나를 맡는다: 폴더 만들기와 연결 열기만 한다. SQL은 모른다
// 몬스터·플레이어 DataSource는 같은 game.db를, 랭킹 DataSource는 Ranking.db를 나눠 쓴다
public class SqliteDatabase
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public SqliteDatabase(string dbPath)
    {
        _dbPath = dbPath;
        _connectionString = $"Data Source={dbPath}";
    }

    // DB 파일이 들어갈 폴더가 없으면 만든다 (파일은 처음 열 때 만들어진다)
    public void EnsureFolder()
    {
        string? folder = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }
    }

    public async Task<SqliteConnection> OpenAsync()
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }
}
