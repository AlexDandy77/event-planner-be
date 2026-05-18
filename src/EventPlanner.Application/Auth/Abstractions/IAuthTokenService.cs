using EventPlanner.Application.Auth.Models;
using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Auth.Abstractions;

public interface IAuthTokenService
{
    AuthToken CreateToken(User user);
}
