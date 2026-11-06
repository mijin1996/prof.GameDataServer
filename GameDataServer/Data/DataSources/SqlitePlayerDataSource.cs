using GameDataServer.Dtos;
using Microsoft.Data.Sqlite;

namespace GameDataServer.Data.DataSources;

// game.db의 players 표와 직접 SQL로 이야기하는 클래스
public class SqlitePlayerDataSource : IPlayerDataSource
{
    private readonly SqliteDatabase _db;

    public SqlitePlayerDataSource(SqliteDatabase db)
    {
        _db = db;
    }

    public async Task CreateTableAsync()
    {
        _db.EnsureFolder();

        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        // 이름은 겹치면 안 되고(UNIQUE) 비밀번호는 해시로만 저장한다
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS players (
                id            INTEGER PRIMARY KEY AUTOINCREMENT,
                name          TEXT    NOT NULL UNIQUE,
                password_hash TEXT    NOT NULL,
                created_at    TEXT    NOT NULL DEFAULT CURRENT_TIMESTAMP
            )
            """;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<PlayerDto?> GetByNameAsync(string name)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name, password_hash FROM players WHERE name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new PlayerDto
        {
            Name = reader.GetString(0),
            PasswordHash = reader.GetString(1),
        };
    }

    public async Task InsertAsync(PlayerDto dto)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO players (name, password_hash) VALUES ($name, $hash)";
        cmd.Parameters.AddWithValue("$name", dto.Name ?? "");
        cmd.Parameters.AddWithValue("$hash", dto.PasswordHash ?? "");
        await cmd.ExecuteNonQueryAsync();
    }
}
