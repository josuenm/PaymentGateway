namespace Auth.Application.Auth.DTOs.Requests;

public record RegisterRequest(
    string Name,
    string Email,
    string Password
);