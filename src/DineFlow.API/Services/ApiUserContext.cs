using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using DineFlow.Application.Common;

namespace DineFlow.API.Services
{
    public class ApiUserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true)
                    return null;

                var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? httpContext.User.FindFirst("sub")?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                    return userId;

                return null;
            }
        }
    }
}
