using System;
using DatingAppAPI.Entities;

namespace DatingAppAPI.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser appUser);
}
