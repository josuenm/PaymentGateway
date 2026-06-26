namespace Auth.Application.Users.DTOs.Responses;

public record UserResponse(
    string Id,  
    string Name,
    string Email
);