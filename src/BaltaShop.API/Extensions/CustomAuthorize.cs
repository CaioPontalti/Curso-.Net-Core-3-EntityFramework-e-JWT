using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BaltaShop.API.Extensions
{
    public class CustomAuthorize
    {
        public class RequisitoClaimFilter : IAuthorizationFilter
        {
            private readonly Claim _claim;

            public RequisitoClaimFilter(Claim claim)
            {
                _claim = claim;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new StatusCodeResult(401);
                    return;
                }

                if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Value))
                {
                    context.Result = new StatusCodeResult(403);
                }
            }
        }

        public class CustomAuthorization
        {
            public static bool ValidarClaimsUsuario(HttpContext context, string claimValue)
            {
                return context.User.Identity.IsAuthenticated &&
                       context.User.Claims.Any(c => c.Value.Contains(claimValue));
            }

        }

        /* O Sufixo 'Attribute' do nome 'ClaimsAuthorizeAttribute' é padrão. 
           O nome ClaimsAuthorize é colocado em cima da Controller */
        public class ClaimsAuthorizeAttribute : TypeFilterAttribute
        {
            public ClaimsAuthorizeAttribute(string claimValue) : base(typeof(RequisitoClaimFilter))
            {
                Arguments = new object[] { new Claim(ClaimTypes.Role, claimValue) };
            }
        }
    }
}
