using Auth.Application.Users.DTOs.Responses;

namespace Auth.Application.Auth.DTOs.Responses;

public record AuthenticationResponse(
    string AccessToken, 
    string RefreshToken, 
    long ExpiresIn,
    UserResponse User
);