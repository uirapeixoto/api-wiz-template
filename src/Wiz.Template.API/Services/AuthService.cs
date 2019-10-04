using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Wiz.Template.API.Services.Interfaces;
using Wiz.Template.API.ViewModels;

namespace Wiz.Template.API.Services
{
    public class AuthService : IAuthService
    {
        public TokenAttributeViewModel GetTokenAttributes(IEnumerable<Claim> userClaims)
        {
            var tokenAttributes = new TokenAttributeViewModel();
            if (userClaims.Count() > 0)
            {
                var r = userClaims.FirstOrDefault(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var s = userClaims.Where((item, index) => index.Equals(3)).FirstOrDefault().Value;

                tokenAttributes = new TokenAttributeViewModel
                {
                    UserId = r,
                    AccessKey = s
                };
            }
            else
                return null;

            return tokenAttributes;
        }
    }
}
