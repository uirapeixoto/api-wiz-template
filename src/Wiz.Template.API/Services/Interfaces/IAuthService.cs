using System.Collections.Generic;
using System.Security.Claims;
using Wiz.Template.API.ViewModels;

namespace Wiz.Template.API.Services.Interfaces
{
    public interface IAuthService
    {
        TokenAttributeViewModel GetTokenAttributes(IEnumerable<Claim> userClaims);
    }
}
