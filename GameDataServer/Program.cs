using GameDataServer.Auth;
using GameDataServer.Data.DataSources;
using GameDataServer.Data.Repositories;
using GameDataServer.Endpoints;
using GameDataServer.Services;
using Microsoft.AspNetCore.Identity;

// Program.cs는 "조립"만 한다: 부품을 등록하고, DB를 준비하고, 주소를 연결하고, 서버를 켠다
// 흐름: Endpoints(주소) → Service(규칙) → Repository(데이터 골라 주기) → DataSource(DB·파일과 통신)

var builder = WebApplication.CreateBuilder(args);

// ── 1. 파일 위치 ─────────────────────────────────────────────────────────
string root = builder.Environment.ContentRootPath;
string gameDbPath = Path.Combine(root, "AppData", "game.db");                // 몬스터·플레이어 DB
string monsterSeedPath = Path.Combine(root, "AppData", "monsters.json");      // 처음 넣을 씨앗 데이터
string rankingDbPath = Path.Combine(root, "RankingData", "Ranking.db");      // 랭킹 DB

var gameDb = new SqliteDatabase(gameDbPath);
var rankingDb = new SqliteDatabase(rankingDbPath);

// ── 2. 부품 등록 (생성자 주입) — 필요한 곳에 알아서 넣어 준다 ────────────
// DataSource
builder.Services.AddSingleton<IMonsterDataSource>(new SqliteMonsterDataSource(gameDb));
builder.Services.AddSingleton<IPlayerDataSource>(new SqlitePlayerDataSource(gameDb));
builder.Services.AddSingleton<IMonsterSeedDataSource>(new JsonFileMonsterSeedDataSource(monsterSeedPath));
builder.Services.AddSingleton<IRankingDataSource>(new SqliteRankingDataSource(rankingDb));

// Repository
builder.Services.AddSingleton<IMonsterRepository, MonsterRepository>();
builder.Services.AddSingleton<IPlayerRepository, PlayerRepository>();
builder.Services.AddSingleton<IRankingRepository, RankingRepository>();

// Service
builder.Services.AddSingleton<IMonsterService, MonsterService>();
builder.Services.AddSingleton<IRankingService, RankingService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();   // 비밀번호를 해시로 바꾼다

// 인증 — 서명 키는 환경 변수 JWT_KEY (없으면 내 PC용 키)
builder.Services.AddJwtAuthentication(JwtSettings.FromEnvironment());

var app = builder.Build();

app.UseAuthentication();   // 요청마다 Authorization 헤더의 토큰을 읽고 검사한다
app.UseAuthorization();    // RequireAuthorization()이 붙은 주소는 검사를 통과해야 들어간다

// ── 3. DB 준비 — 테이블이 없으면 만들고, 몬스터가 비어 있으면 씨앗 데이터를 넣는다 ──
await app.Services.GetRequiredService<IMonsterRepository>().InitializeAsync();
await app.Services.GetRequiredService<IPlayerRepository>().InitializeAsync();
await app.Services.GetRequiredService<IRankingRepository>().InitializeAsync();

// ── 4. 주소 연결 ─────────────────────────────────────────────────────────
app.MapGet("/hello", () => "안녕하세요, 서버입니다.");
app.MapAuthEndpoints();      // /auth ...
app.MapMonsterEndpoints();   // /monsters ...
app.MapRankingEndpoints();   // /ranking ...

// ── 5. 서버 켜기 — 배포 환경은 PORT 환경 변수로 포트를 알려 준다 ─────────
string port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
