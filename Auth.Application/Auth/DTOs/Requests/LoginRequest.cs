namespace Auth.Application.Auth.DTOs.Requests;

public record LoginRequest(
    string Email,
    string Password
);