# API 주소 목록

> 분류: Reference (찾아보는 정보)

- 로컬 주소: `http://localhost:5000`
- 배포 주소: `https://gamedataserver-q5ig.onrender.com`

## 기본

| 메서드 | 주소 | 설명 | 성공 응답 |
|---|---|---|---|
| GET | `/hello` | 서버가 켜져 있는지 확인 | 200, 문자열 |

## 몬스터

| 메서드 | 주소 | 설명 | 성공 응답 | 실패 응답 |
|---|---|---|---|---|
| GET | `/monsters` | 전체 조회 | 200 | - |
| GET | `/monsters/{id}` | 하나 조회 | 200 | 404 없는 id |
| POST | `/monsters` | 추가 | 201 Created | 400 이름이 비어 있음 |
| PUT | `/monsters/{id}` | 고치기 | 200 | 400 이름이 비어 있음, 404 없는 id |
| DELETE | `/monsters/{id}` | 삭제 | 204 No Content | 404 없는 id |
| GET | `/monsters/slow?ms=3000` | 테스트용: 일부러 늦게 대답 (기본 3000ms) | 200 | - |
| GET | `/monsters/big?count=20000` | 테스트용: 일부러 큰 목록 (기본 20000마리) | 200 | - |

### 몬스터 JSON 모양

POST·PUT 요청 본문과 응답이 같은 모양입니다.

```json
{ "id": 1, "name": "일반 좀비", "hp": 100, "attack": 20, "speed": 2.0 }
```

- `name`은 비어 있으면 안 됩니다.
- 빠진 값은 기본값(숫자 0, 문자 `""`)으로 채워집니다.

### 목록 응답 모양

`/monsters`, `/monsters/slow`, `/monsters/big`은 배열을 `monsters`로 감싸서 돌려줍니다.

```json
{ "monsters": [ { "id": 1, "name": "일반 좀비", "hp": 100, "attack": 20, "speed": 2.0 } ] }
```

### 실패 응답 모양

```json
{ "error": "id 99 몬스터가 없습니다." }
```

## 랭킹

| 메서드 | 주소 | 설명 | 성공 응답 | 실패 응답 |
|---|---|---|---|---|
| POST | `/ranking` | 점수 저장 | 200 | 400 이름이 비어 있음 |
| GET | `/ranking/top` | 상위 10명 조회 | 200 | - |

### 규칙

- `id`는 플레이어 이름이고, 비어 있으면 안 됩니다.
- 같은 이름으로 다시 저장하면 **더 높은 점수만** 남습니다.

### 요청 / 응답 모양

```json
{ "id": "player1", "score": 1200 }
```

`/ranking/top` 응답:

```json
{ "ranking": [ { "id": "player1", "score": 1200 } ] }
```
