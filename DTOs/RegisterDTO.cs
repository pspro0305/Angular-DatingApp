using System;
using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.DTOs;

public class RegisterDTO
{
    [MaxLength(100)]
    public required string UserName {get;set;}
    public required string Password {get;set;}
}
