using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppAPI.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser appUser)
    {
       var tokenkey = config["TokenKey"] ?? throw new Exception("Cannot access Token Key from appsettings");
       if(tokenkey.Length<64) throw new Exception("Your token key needs to be longer");
       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey));

       var claims = new List<Claim>
       {
        new(ClaimTypes.NameIdentifier,appUser.UserName),
        new(ClaimTypes.Role,"Admin") // added just for testing purpose
       };

       var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
       // Note : When we use above algo then the key length needs to be greater than 64 characters
       var tokendescriptor = new SecurityTokenDescriptor
       {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = creds
       };

       var tokenhandler = new JwtSecurityTokenHandler();
       var token = tokenhandler.CreateToken(tokendescriptor);

       return tokenhandler.WriteToken(token);
    }
}
