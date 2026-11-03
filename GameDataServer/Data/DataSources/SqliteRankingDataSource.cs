using GameDataServer.Dtos;
using Microsoft.Data.Sqlite;

namespace GameDataServer.Data.DataSources;

// RankingData/Ranking.db 파일과 직접 SQL로 이야기하는 클래스
public class SqliteRankingDataSource : IRankingDataSource
{
    private readonly string _dbPath;
    private readonly string _connectionString;

    public SqliteRankingDataSource(string dbPath)
    {
        _dbPath = dbPath;
        _connectionString = $"Data Source={dbPath}";
    }

    public async Task CreateTableAsync()
    {
        // RankingData 폴더가 없으면 만든다 (예전엔 수작업이 필요했던 부분)
        string? folder = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        await using SqliteConnection conn = await OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS ranking (
                id     TEXT    PRIMARY KEY,
                score  INTEGER NOT NULL
            )
            """;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpsertBestScoreAsync(RankingDto dto)
    {
        await using SqliteConnection conn = await OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO ranking (id, score) VALUES ($id, $score)
            ON CONFLICT(id) DO UPDATE SET score = MAX(score, excluded.score)
            """;
        cmd.Parameters.AddWithValue("$id", dto.Id ?? "");
        cmd.Parameters.AddWithValue("$score", dto.Score ?? 0);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<RankingDto>> GetTopAsync(int limit)
    {
        await using SqliteConnection conn = await OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, score FROM ranking ORDER BY score DESC LIMIT $limit";
        cmd.Parameters.AddWithValue("$limit", limit);
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();

        var list = new List<RankingDto>();
        while (await reader.ReadAsync())
        {
            list.Add(new RankingDto
            {
                Id = reader.GetString(0),
                Score = reader.GetInt32(1),
            });
        }
        return list;
    }

    private async Task<SqliteConnection> OpenAsync()
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }
}
