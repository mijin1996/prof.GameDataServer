# Render에 배포하기

> 분류: How-to (특정 작업 방법)

이 서버는 [Render](https://render.com)에 Docker 방식 웹 서비스로 배포되어 있습니다.

- 배포 주소: `https://gamedataserver-q5ig.onrender.com`

## 배포 방법

GitHub `master` 브랜치에 푸시하면 Render가 자동으로 다시 배포합니다(Auto-Deploy).

```bash
git add -A
git commit -m "변경 내용"
git push
```

Render 대시보드의 **Deploys** 목록 맨 위에 새 배포가 생기고, 초록 체크(Live)가 되면 완료입니다.

## 토큰 서명 키 넣기 (처음 한 번)

로그인 토큰은 서명 키로 도장을 찍습니다. 코드에 들어 있는 키는 내 PC용이라 배포 서버에서는 따로 넣어야 합니다.

1. Render 대시보드 → 서비스 → **Environment**
2. **Add Environment Variable**
   - Key: `JWT_KEY`
   - Value: 32글자 이상의 아무도 모르는 문자열
3. 저장하면 다시 배포됩니다.

키가 32바이트보다 짧으면 서버가 켜지지 않고 `JWT_KEY는 32바이트 이상이어야 합니다.` 오류를 냅니다.

## 배포가 동작하는 방식

1. Render가 저장소 루트의 `Dockerfile`로 이미지를 만듭니다.
2. `Dockerfile`은 본 프로젝트(`GameDataServer/GameDataServer.csproj`)만 publish 합니다. 테스트 프로젝트는 포함되지 않습니다.
3. Render가 `PORT` 환경 변수로 포트를 알려 주고, 서버는 그 포트로 켜집니다. 값이 없으면 5000을 씁니다.

## 배포 확인

```
https://gamedataserver-q5ig.onrender.com/hello
```

## 알아 둘 점

- 무료 플랜은 한동안 요청이 없으면 서버가 잠들어서, 첫 요청이 50초 이상 걸릴 수 있습니다. 꺼진 것이 아니니 기다리면 됩니다.
- DB 파일(`*.db`)은 저장소에 올리지 않기 때문에, 배포 서버는 `monsters.json`으로 새로 시작합니다. 배포 서버에서 추가한 몬스터, 랭킹, 가입한 플레이어는 다시 배포하면 사라질 수 있습니다.
