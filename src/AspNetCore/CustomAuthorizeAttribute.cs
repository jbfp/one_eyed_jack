using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sequence.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var headers = context.HttpContext.Request.Headers;

            if (headers.TryGetValue("Authorization", out var values) && values.Count > 0)
            {
                return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
