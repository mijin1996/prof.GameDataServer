namespace GameDataServer.Dtos;

// POST /auth/login 응답 JSON: { "accessToken": "...", "tokenType": "Bearer", "expiresIn": 1800 }
// Unity의 TokenResponse 클래스와 이름을 맞췄다
public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
