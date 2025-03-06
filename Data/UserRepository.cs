using System;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingAppAPI.Data;

public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{
    public async Task<MemberDTO?> GetMemberAsync(string username)
    {
        return await context.Users
        .Where(x=>x.UserName == username)
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await context.Users
        .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users
        .FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
       return await context.Users
       .Include(p=>p.Photos)
       .SingleOrDefaultAsync(x=>x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users
        .Include(p=>p.Photos)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
