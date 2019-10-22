using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace TRS.Extensions
{
    public static class ClaimsExtensions
    {
        static string GetUserEmail(this ClaimsIdentity identity)
        {
            return identity.Claims?.FirstOrDefault(c => c.Type == "TRS.Models.RegisterViewModel.Email")?.Value;
        }

        public static string GetUserEmail(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity != null ? GetUserEmail(claimsIdentity) : "";
        }

        static string GetUserNameIdentifier(this ClaimsIdentity identity)
        {
            return identity.Claims?.FirstOrDefault(c => c.Type == "TRS.Models.RegisterViewModel.NameIdentifier")?.Value;
        }

        public static string GetUserNameIdentifier(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity != null ? GetUserNameIdentifier(claimsIdentity) : "";
        }

        public static int GetCustomerId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CustomerId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? int.Parse(claim.Value) : -1;

        }

        public static string GetRoles(this IIdentity identity)
        {

            var claim = ((ClaimsIdentity)identity).FindFirst("Roles");
            // Test for null to avoid issues during local testing
            return claim.Value;

        }


    }
}