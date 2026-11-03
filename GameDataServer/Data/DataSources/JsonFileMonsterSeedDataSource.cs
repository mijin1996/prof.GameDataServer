using System.Text.Json;
using GameDataServer.Dtos;

namespace GameDataServer.Data.DataSources;

// AppData/monsters.json 파일을 읽어서 DTO 목록으로 돌려준다
public class JsonFileMonsterSeedDataSource : IMonsterSeedDataSource
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,   // "name"과 Name을 같은 것으로 본다
    };

    private readonly string _path;

    public JsonFileMonsterSeedDataSource(string path)
    {
        _path = path;
    }

    public async Task<List<MonsterDto>> LoadAsync()
    {
        if (!File.Exists(_path))
        {
            return new List<MonsterDto>();
        }

        string json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<List<MonsterDto>>(json, JsonOptions) ?? new List<MonsterDto>();
    }
}
