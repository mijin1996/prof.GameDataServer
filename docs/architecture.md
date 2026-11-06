# 구조 설명

> 분류: Explanation (왜 이렇게 했는지)

## 솔루션 구조

솔루션은 본 프로젝트와 테스트 프로젝트로 분리되어 있습니다.

```
prof.GameDataServer/
├── GameDataServer.slnx       두 프로젝트를 묶는 솔루션
├── GameDataServer/           본 프로젝트 (서버)
├── GameDataServer.Tests/     테스트 프로젝트 (xUnit)
├── docs/                     문서
└── Dockerfile                배포용
```

테스트를 따로 두면 배포할 때 테스트 코드가 섞이지 않고, 서버 코드와 검증 코드의 역할이 분명해집니다.

## 프로젝트 내부: 레이어드 아키텍처

요청은 위에서 아래로만 흐릅니다. 아래 계층은 위 계층을 모릅니다.

```
요청 → Endpoints → Services → Data/Repositories → Data/DataSources → DB·파일
```

| 계층 | 폴더 | 하는 일 | 모르는 것 |
|---|---|---|---|
| 표현 | `Endpoints/` | 주소를 Service에 연결하고, 결과를 HTTP 응답(200, 404 ...)으로 바꿈 | DB |
| 비즈니스 | `Services/` | 입력 검증과 게임 규칙 (이름은 비면 안 됨, 랭킹은 최고 점수만, 로그인 규칙) | DB, HTTP |
| 데이터 | `Data/Repositories/` | DataSource를 감추고 Model로 돌려줌, 데이터 실패를 Result로 | HTTP, 규칙 |
| 데이터 | `Data/DataSources/` | SQLite·JSON 파일과 직접 통신 | 규칙 |

계층 사이에서 주고받는 데이터와 공통 부품은 따로 둡니다.

| 폴더 | 역할 |
|---|---|
| `Models/` | 서버 안에서 쓰는 데이터. `record`, null 없음 |
| `Dtos/` | 바깥(요청 JSON, DB 한 줄)에서 온 날것의 데이터. 전부 nullable |
| `Mappers/` | DTO ↔ Model 변환, 빠진 값은 기본값으로 |
| `Core/` | 공통 `Result`, `AppError` |
| `Auth/` | JWT 설정(`JwtSettings`), 토큰 발급(`JwtTokenService`), 토큰 검사 등록(`AuthExtensions`) |

이렇게 나눈 이유는 **관심사의 분리**입니다. 예를 들어 DB를 SQLite에서 다른 것으로 바꿔도 DataSource만 고치면 되고, Service와 Endpoints는 그대로입니다.

## 그림으로 보기

레이어 구조, 데이터 모양, 로그인·몬스터 추가·랭킹 흐름, 배포 구조를 UML 그림으로 정리했습니다 → [uml.md](uml.md)

## 인증: JWT

로그인하면 서버가 서명한 토큰(JWT)을 주고, 쓰기 요청(몬스터 추가·수정·삭제)은 그 토큰이 있어야 들어옵니다.

```
POST /auth/login → AuthEndpoints → AuthService → PlayerRepository → SqlitePlayerDataSource (players 표)
                                        └→ ITokenService(JwtTokenService)가 토큰 발급
```

- 비밀번호는 원문을 저장하지 않고 `PasswordHasher`로 만든 해시만 `players` 표에 저장합니다.
- 수업용 간소화: 처음 보는 이름으로 로그인하면 그 비밀번호로 가입됩니다.
- 토큰 검사는 ASP.NET Core가 합니다. `RequireAuthorization()`이 붙은 주소는 토큰이 없거나 틀리면 401을 돌려줍니다.
- 서명 키는 환경 변수 `JWT_KEY`에서 읽고, 없으면 내 PC용 키를 씁니다.

## 실패 처리: Result 패턴

예외를 던지는 대신 `Result<T>.Success` 또는 `Result<T>.Failure(AppError)`를 돌려줍니다.

- 데이터 실패(`NotFound`)는 Repository가 만듭니다.
- 규칙 실패(`InvalidName`, `InvalidLogin`, `WrongPassword`)는 Service가 만듭니다.
- `Endpoints/ErrorResults.cs` 한 곳에서 이것을 404, 400, 401 같은 HTTP 응답으로 바꿉니다.

## 부품 연결: 생성자 주입

`Program.cs`는 "조립"만 합니다. 각 클래스는 필요한 부품을 인터페이스로 생성자에서 받기 때문에, 테스트할 때 가짜(Fake)로 바꿔 끼울 수 있습니다. `GameDataServer.Tests/Services/`의 테스트가 그 예입니다.

## SOLID 원칙이 적용된 곳

| 원칙 | 적용한 곳 |
|---|---|
| S 단일 책임 | 계층마다 하는 일이 하나. DB 파일 열기는 `SqliteDatabase`, SQL은 각 DataSource, 토큰 만들기는 `JwtTokenService` |
| O 개방-폐쇄 | 새 실패 종류는 `AppError`와 `ErrorResults`에만 추가. 각 Endpoints 파일은 고치지 않음 |
| L 리스코프 치환 | `IMonsterRepository` 자리에 진짜(`MonsterRepository`)든 테스트용 가짜든 똑같이 동작 |
| I 인터페이스 분리 | 몬스터·씨앗 데이터·랭킹·플레이어 DataSource를 각각 작은 인터페이스로 나눔 |
| D 의존성 역전 | Endpoints → `IMonsterService`, Service → `IMonsterRepository`·`ITokenService`처럼 구체 클래스가 아니라 인터페이스에 기댐 |

## 데이터 파일 위치

| 경로 | 내용 |
|---|---|
| `GameDataServer/AppData/monsters.json` | 처음 넣을 몬스터 데이터 (저장소에 포함) |
| `GameDataServer/AppData/game.db` | 몬스터·플레이어 DB (실행하면 생성, 저장소에 안 올림) |
| `GameDataServer/RankingData/Ranking.db` | 랭킹 DB (실행하면 생성, 저장소에 안 올림) |

코드 폴더 `Data/`와 헷갈리지 않도록 데이터 파일 폴더는 `AppData/`로 이름을 붙였습니다.
