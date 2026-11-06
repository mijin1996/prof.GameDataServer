# API 주소 목록

> 분류: Reference (찾아보는 정보)

- 로컬 주소: `http://localhost:5000`
- 배포 주소: `https://gamedataserver-q5ig.onrender.com`
- 🔒 표시가 있는 주소는 로그인 토큰이 필요합니다. 요청 헤더에 `Authorization: Bearer <토큰>`을 붙입니다.

## 기본

| 메서드 | 주소 | 설명 | 성공 응답 |
|---|---|---|---|
| GET | `/hello` | 서버가 켜져 있는지 확인 | 200, 문자열 |

## 인증

| 메서드 | 주소 | 설명 | 성공 응답 | 실패 응답 |
|---|---|---|---|---|
| POST | `/auth/login` | 로그인하고 토큰 받기 (처음 보는 이름이면 가입) | 200 | 400 비어 있음, 401 비밀번호 틀림 |
| GET | `/auth/me` 🔒 | 토큰의 주인 확인 | 200 | 401 토큰 없음·틀림·만료 |

### 요청 / 응답 모양

`/auth/login` 요청:

```json
{ "player": "kim", "password": "1234" }
```

`/auth/login` 응답 (토큰은 30분 동안 유효):

```json
{ "accessToken": "eyJhbGciOi...", "tokenType": "Bearer", "expiresIn": 1800 }
```

`/auth/me` 응답:

```json
{ "player": "kim", "expiresAt": "2026-11-06T05:00:00+00:00" }
```

## 몬스터

| 메서드 | 주소 | 설명 | 성공 응답 | 실패 응답 |
|---|---|---|---|---|
| GET | `/monsters` | 전체 조회 | 200 | - |
| GET | `/monsters/{id}` | 하나 조회 | 200 | 404 없는 id |
| POST | `/monsters` 🔒 | 추가 | 201 Created | 400 이름이 비어 있음, 401 토큰 없음 |
| PUT | `/monsters/{id}` 🔒 | 고치기 | 200 | 400 이름이 비어 있음, 401 토큰 없음, 404 없는 id |
| DELETE | `/monsters/{id}` 🔒 | 삭제 | 204 No Content | 401 토큰 없음, 404 없는 id |
| GET | `/monsters/slow?ms=3000` | 테스트용: 일부러 늦게 대답 (기본 3000ms, 0~30000) | 200 | - |
| GET | `/monsters/big?count=20000` | 테스트용: 일부러 큰 목록 (기본 20000마리, 0~100000) | 200 | - |

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

토큰이 없어서 막힌 401은 본문 없이 `WWW-Authenticate: Bearer` 헤더만 옵니다.

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
