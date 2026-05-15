using PayDa.Domain.Entities;

namespace PayDa.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
