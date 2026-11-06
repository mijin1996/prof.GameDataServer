using GameDataServer.Dtos;
using Microsoft.Data.Sqlite;   // SQLite 데이터베이스를 여는 도구

namespace GameDataServer.Data.DataSources;

// AppData/game.db의 monsters 표와 직접 SQL로 이야기하는 클래스. 몬스터 SQL은 여기에만 있다
public class SqliteMonsterDataSource : IMonsterDataSource
{
    private const string SelectColumns = "SELECT id, name, hp, attack, speed FROM monsters";

    private readonly SqliteDatabase _db;

    public SqliteMonsterDataSource(SqliteDatabase db)
    {
        _db = db;
    }

    public async Task CreateTableAsync()
    {
        _db.EnsureFolder();

        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS monsters (
                id     INTEGER PRIMARY KEY AUTOINCREMENT,
                name   TEXT    NOT NULL,
                hp     INTEGER NOT NULL,
                attack INTEGER NOT NULL,
                speed  REAL    NOT NULL
            )
            """;
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<long> CountAsync()
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM monsters";
        object? count = await cmd.ExecuteScalarAsync();
        return count is long n ? n : 0;
    }

    public async Task<List<MonsterDto>> GetAllAsync()
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = SelectColumns + " ORDER BY id";
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        return await ReadRowsAsync(reader);
    }

    public async Task<MonsterDto?> GetByIdAsync(int id)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = SelectColumns + " WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);   // 값은 글자로 이어 붙이지 않고 파라미터로 넘긴다
        await using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        List<MonsterDto> rows = await ReadRowsAsync(reader);
        return rows.Count == 0 ? null : rows[0];
    }

    public async Task<int> InsertAsync(MonsterDto dto)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO monsters (name, hp, attack, speed) VALUES ($name, $hp, $attack, $speed);
            SELECT last_insert_rowid();
            """;
        AddMonsterParameters(cmd, dto);
        object? newId = await cmd.ExecuteScalarAsync();   // DB가 붙여 준 새 id
        return (int)(long)newId!;
    }

    public async Task<int> UpdateAsync(int id, MonsterDto dto)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE monsters SET name = $name, hp = $hp, attack = $attack, speed = $speed WHERE id = $id";
        AddMonsterParameters(cmd, dto);
        cmd.Parameters.AddWithValue("$id", id);
        return await cmd.ExecuteNonQueryAsync();   // 바뀐 줄 수. 그 id가 없으면 0
    }

    public async Task<int> DeleteAsync(int id)
    {
        await using SqliteConnection conn = await _db.OpenAsync();
        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM monsters WHERE id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        return await cmd.ExecuteNonQueryAsync();   // 지운 줄 수
    }

    // ---- 여기부터는 이 클래스 안에서만 쓰는 도우미 ----

    private static void AddMonsterParameters(SqliteCommand cmd, MonsterDto dto)
    {
        cmd.Parameters.AddWithValue("$name", dto.Name ?? "");
        cmd.Parameters.AddWithValue("$hp", dto.Hp ?? 0);
        cmd.Parameters.AddWithValue("$attack", dto.Attack ?? 0);
        cmd.Parameters.AddWithValue("$speed", dto.Speed ?? 0f);
    }

    // SELECT 결과(행들)를 DTO 목록으로 바꾼다
    private static async Task<List<MonsterDto>> ReadRowsAsync(SqliteDataReader reader)
    {
        var list = new List<MonsterDto>();
        while (await reader.ReadAsync())   // 한 줄(한 마리)씩
        {
            list.Add(new MonsterDto
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Hp = reader.GetInt32(2),
                Attack = reader.GetInt32(3),
                Speed = reader.GetFloat(4),
            });
        }
        return list;
    }
}
