using FileFlowApi.Models;

namespace FileFlowApi.IREPOSITORY
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
