# GameDataServer

SQLite 기반 게임 데이터 서버입니다. 몬스터 데이터와 랭킹을 HTTP API로 제공합니다.

## 문서

| 문서 | 내용 |
|---|---|
| [처음 실행해 보기](docs/getting-started.md) | 서버 켜기, 응답 확인, 테스트 실행 |
| [Render에 배포하기](docs/deploy-render.md) | 자동 배포 방법과 주의할 점 |
| [API 주소 목록](docs/api.md) | 몬스터·랭킹 주소와 JSON 모양 |
| [구조 설명](docs/architecture.md) | 레이어드 아키텍처와 폴더 역할 |

## 참고 도구

### 서버에 요청을 보내서 응답을 확인하는 API 테스트 도구
- https://www.postman.com/downloads/

### SQL(추가 및 등록, 삭제 등의 작업관리 = 서버 없이 파일 하나(game.db 같은 것)에 데이터를 저장하고 꺼내 쓰게 해 주는 가벼운 데이터베이스)
- https://www.sqlite.org/
- 
### .db 파일을 내가 직접 열어 보고 관리하게 도와주는 프로그램
- https://sqlitebrowser.org/dl/
