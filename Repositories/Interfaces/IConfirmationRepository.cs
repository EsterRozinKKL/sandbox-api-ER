using System.Collections.Generic;
using System.Threading.Tasks;

namespace sandboxEr.Repositories.Interfaces
{
    public interface IGetToken
    {
       Task<OAuth2Response> GenerateToken(TokenParam token);
       Task<string> GenerateCode();
    }
}