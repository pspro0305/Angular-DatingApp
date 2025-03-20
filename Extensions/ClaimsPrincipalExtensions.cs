using System;
using System.Security.Claims;

namespace DatingAppAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
   public static string GetUsername(this ClaimsPrincipal user)
   {
       var username = user.FindFirstValue(ClaimTypes.NameIdentifier);
       if(username==null) throw new Exception("User not found in token");
       return username;
   }
}
