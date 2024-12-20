using System;
using System.Security.Cryptography;
using System.Text;
using DatingAppAPI.Data;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName))
        {
            return BadRequest("User exists already!");
        }
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDTO.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDTO{Username = user.UserName, Token = tokenService.CreateToken(user)};
    } 

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
      var user = await context.Users.FirstOrDefaultAsync(
        x=>x.UserName == loginDTO.UserName.ToLower());
        if(user == null){
            return Unauthorized("Invalid Username");
        }

        var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if(computedHash[i]!= user.PasswordHash[i]){
                return Unauthorized("Invalid Password");
            }
        }
        return new UserDTO{Username = user.UserName,  Token = tokenService.CreateToken(user) };
    }
    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x=>x.UserName.ToLower() == username.ToLower());
    }
}
