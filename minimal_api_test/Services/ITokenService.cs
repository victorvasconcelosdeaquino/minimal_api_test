using minimal_api_test.Entities;

namespace minimal_api_test.Services;

public interface ITokenService
{
    string GenerateToken(string key, string issuer, string audience, UserModel user);
}
