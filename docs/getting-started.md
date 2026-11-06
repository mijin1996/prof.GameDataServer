# 처음 실행해 보기

> 분류: Tutorial (따라 하며 배우기)

내 PC에서 서버를 켜고, 응답이 오는지 확인하고, 로그인해서 몬스터를 추가하고, 테스트까지 돌려 보는 순서입니다.

## 준비물

- .NET 10 SDK
- VS Code 또는 Visual Studio
- (선택) [Postman](https://www.postman.com/downloads/) — POST·PUT·DELETE 요청을 보낼 때 사용

## 1. 서버 켜기

저장소 루트(`prof.GameDataServer`)에서 실행합니다.

```bash
dotnet run --project GameDataServer
```

터미널에 아래 줄이 보이면 서버가 켜진 것입니다.

```
Now listening on: http://0.0.0.0:5000
Application started. Press Ctrl+C to shut down.
```

처음 켤 때 `GameDataServer/AppData/game.db`와 `GameDataServer/RankingData/Ranking.db`가 자동으로 만들어지고, 몬스터 테이블이 비어 있으면 `AppData/monsters.json`의 몬스터 5마리가 들어갑니다.

이 창은 켜 둔 채로, 아래 요청은 **다른 터미널 창**에서 보냅니다.

## 2. 응답 확인하기

브라우저 주소창에 입력합니다. `0.0.0.0`이 아니라 `localhost`로 열어야 합니다.

| 주소 | 기대 결과 |
|---|---|
| `http://localhost:5000/hello` | `안녕하세요, 서버입니다.` |
| `http://localhost:5000/monsters` | 몬스터 5마리 JSON |
| `http://localhost:5000/ranking/top` | 랭킹 목록 (처음엔 빈 목록) |

`http://localhost:5000/`처럼 뒤가 비어 있으면 404가 나오는데, 연결된 주소가 없어서 그런 것이고 정상입니다.

## 3. 로그인해서 몬스터 추가하기 (PowerShell)

토큰 없이 추가하면 막힙니다.

```powershell
curl.exe -i -X POST http://localhost:5000/monsters
```

`HTTP/1.1 401 Unauthorized`가 나오면 정상입니다.

로그인해서 토큰을 받습니다. 처음 보는 이름이면 그 비밀번호로 가입됩니다.

```powershell
$login = Invoke-RestMethod -Method Post -Uri http://localhost:5000/auth/login -ContentType "application/json" -Body '{"player":"kim","password":"1234"}'
$login.accessToken
```

토큰을 붙여서 내 정보를 확인하고, 몬스터를 추가합니다.

```powershell
$headers = @{ Authorization = "Bearer $($login.accessToken)" }

Invoke-RestMethod -Uri http://localhost:5000/auth/me -Headers $headers

Invoke-RestMethod -Method Post -Uri http://localhost:5000/monsters -Headers $headers -ContentType "application/json" -Body '{"name":"드래곤","hp":500,"attack":80,"speed":3.5}'
```

서버 창에 `몬스터 추가: 드래곤 (추가한 사람: kim)`이 찍힙니다.

서버를 끌 때는 서버 터미널에서 **Ctrl + C**를 누릅니다.

## 4. 테스트 돌려 보기

```bash
dotnet test
```

마지막에 `실패: 0`이 나오면 성공입니다.

## 다음 단계

- 전체 주소 목록 → [api.md](api.md)
- 폴더가 왜 이렇게 나뉘어 있는지 → [architecture.md](architecture.md)
- 인터넷에 배포하기 → [deploy-render.md](deploy-render.md)
