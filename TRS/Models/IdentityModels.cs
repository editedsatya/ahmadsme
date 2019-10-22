
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TRS.Models
{
    public class CustomIdentityUser : IdentityUser
    {
        public string NameIdentifier { get; set; }
        public int CustomerId { get; set; }
        
        
    }
    public class ApplicationUser : CustomIdentityUser
    {
        

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("TRS.Models.RegisterViewModel.NameIdentifier", NameIdentifier));
            userIdentity.AddClaim(new Claim("TRS.Models.RegisterViewModel.Email", Email));


            userIdentity.AddClaim(new Claim("CustomerId", CustomerId.ToString()));
            //   var result = System.String.Join(", ", names.ToArray());

            List<string> Roleslist = manager.GetRoles(userIdentity.GetUserId()) as List<string>;


            userIdentity.AddClaim(new Claim("Roles", System.String.Join(",", Roleslist.ToArray())));

            return userIdentity;
        }
    }

    //public static class IdentityExtensions
    //{
    //    public static int GetCustomerId(this IIdentity identity)
    //    {
    //        var claim = ((ClaimsIdentity)identity).FindFirst("CustomerId");
    //        // Test for null to avoid issues during local testing
    //        return (claim != null) ? int.Parse(claim.Value) : -1;

    //    }

    //    public static string GetRoles(this IIdentity identity)
    //    {
            
    //        var claim = ((ClaimsIdentity)identity).FindFirst("Roles");
    //        // Test for null to avoid issues during local testing
    //        return claim.ToString();

    //    }


    //}




}