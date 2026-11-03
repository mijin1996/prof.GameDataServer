using GameDataServer.Data.DataSources;
using GameDataServer.Endpoints;
using GameDataServer.Data.Repositories;
using GameDataServer.Services;

// Program.cs는 "조립"만 한다: 부품을 등록하고, DB를 준비하고, 주소를 연결하고, 서버를 켠다
// 흐름: Endpoints(주소) → Service(규칙) → Repository(데이터 골라 주기) → DataSource(DB·파일과 통신)

var builder = WebApplication.CreateBuilder(args);

// ── 1. 파일 위치 ─────────────────────────────────────────────────────────
string root = builder.Environment.ContentRootPath;
string monsterDbPath = Path.Combine(root, "AppData", "game.db");             // 몬스터 DB
string monsterSeedPath = Path.Combine(root, "AppData", "monsters.json");      // 처음 넣을 씨앗 데이터
string rankingDbPath = Path.Combine(root, "RankingData", "Ranking.db");   // 랭킹 DB

// ── 2. 부품 등록 (생성자 주입) — 필요한 곳에 알아서 넣어 준다 ────────────
builder.Services.AddSingleton<IMonsterDataSource>(new SqliteMonsterDataSource(monsterDbPath));
builder.Services.AddSingleton<IMonsterSeedDataSource>(new JsonFileMonsterSeedDataSource(monsterSeedPath));
builder.Services.AddSingleton<IRankingDataSource>(new SqliteRankingDataSource(rankingDbPath));

builder.Services.AddSingleton<IMonsterRepository, MonsterRepository>();
builder.Services.AddSingleton<IRankingRepository, RankingRepository>();

builder.Services.AddSingleton<MonsterService>();
builder.Services.AddSingleton<RankingService>();

var app = builder.Build();

// ── 3. DB 준비 — 테이블이 없으면 만들고, 몬스터가 비어 있으면 씨앗 데이터를 넣는다 ──
await app.Services.GetRequiredService<IMonsterRepository>().InitializeAsync();
await app.Services.GetRequiredService<IRankingRepository>().InitializeAsync();

// ── 4. 주소 연결 ─────────────────────────────────────────────────────────
app.MapGet("/hello", () => "안녕하세요, 서버입니다.");
app.MapMonsterEndpoints();   // /monsters ...
app.MapRankingEndpoints();   // /ranking ...

// ── 5. 서버 켜기 — 배포 환경은 PORT 환경 변수로 포트를 알려 준다 ─────────
string port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
