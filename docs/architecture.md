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
| 비즈니스 | `Services/` | 입력 검증과 게임 규칙 (이름은 비면 안 됨, 랭킹은 최고 점수만) | DB, HTTP |
| 데이터 | `Data/Repositories/` | DataSource를 감추고 Model로 돌려줌, 데이터 실패를 Result로 | HTTP, 규칙 |
| 데이터 | `Data/DataSources/` | SQLite·JSON 파일과 직접 통신 | 규칙 |

계층 사이에서 주고받는 데이터는 따로 둡니다.

| 폴더 | 역할 |
|---|---|
| `Models/` | 서버 안에서 쓰는 데이터. `record`, null 없음 |
| `Dtos/` | 바깥(요청 JSON, DB 한 줄)에서 온 날것의 데이터. 전부 nullable |
| `Mappers/` | DTO ↔ Model 변환, 빠진 값은 기본값으로 |
| `Core/` | 공통 `Result`, `AppError` |

이렇게 나눈 이유는 **관심사의 분리**입니다. 예를 들어 DB를 SQLite에서 다른 것으로 바꿔도 DataSource만 고치면 되고, Service와 Endpoints는 그대로입니다.

## 실패 처리: Result 패턴

예외를 던지는 대신 `Result<T>.Success` 또는 `Result<T>.Failure(AppError)`를 돌려줍니다.

- 데이터 실패(`NotFound`)는 Repository가 만듭니다.
- 규칙 실패(`InvalidName`)는 Service가 만듭니다.
- Endpoints가 이것을 404, 400 같은 HTTP 응답으로 바꿉니다.

## 부품 연결: 생성자 주입

`Program.cs`는 "조립"만 합니다. 각 클래스는 필요한 부품을 인터페이스로 생성자에서 받기 때문에, 테스트할 때 가짜(Mock)로 바꿔 끼울 수 있습니다.

## 데이터 파일 위치

| 경로 | 내용 |
|---|---|
| `GameDataServer/AppData/monsters.json` | 처음 넣을 몬스터 데이터 (저장소에 포함) |
| `GameDataServer/AppData/game.db` | 몬스터 DB (실행하면 생성, 저장소에 안 올림) |
| `GameDataServer/RankingData/Ranking.db` | 랭킹 DB (실행하면 생성, 저장소에 안 올림) |

코드 폴더 `Data/`와 헷갈리지 않도록 데이터 파일 폴더는 `AppData/`로 이름을 붙였습니다.
