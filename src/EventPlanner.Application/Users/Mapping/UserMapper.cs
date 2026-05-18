using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Users.Dtos;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Application.Users.Mapping;

internal static class UserMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            ToContractValue(user.Role),
            user.Phone,
            user.CreatedAt,
            user.UpdatedAt
        );
    }

    private static string ToContractValue(UserRole role)
    {
        return role switch
        {
            UserRole.Organizer => "organizer",
            UserRole.Admin => "admin",
            _ => throw new ApplicationValidationException("Unsupported user role.")
        };
    }
}
