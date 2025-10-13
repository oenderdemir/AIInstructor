namespace AIInstructor.src.Shared.Attributes
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public PermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userPermissions = user.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();

            bool isAdmin = userPermissions.Contains("KullaniciTipi.Admin");

            bool hasAnyPermission = _permissions.Any(p => userPermissions.Contains(p));

            if (!(isAdmin || hasAnyPermission))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
