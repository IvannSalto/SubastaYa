using System.Security.Claims;

namespace SubastaYa.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            
            if (claim == null)
            {
                throw new UnauthorizedAccessException("El token no contiene un ID de usuario.");
            }

            return int.Parse(claim.Value);
        }
    }
}