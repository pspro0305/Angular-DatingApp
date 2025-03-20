using System.Security.Claims;
using AutoMapper;
using DatingAppAPI.DTOs;
using DatingAppAPI.Entities;
using DatingAppAPI.Extensions;
using DatingAppAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppAPI.Controllers
{
   [Authorize]
    public class UsersController(IUserRepository userRepository, 
        IMapper mapper, IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();
            return Ok(users);
        }
         [HttpGet,Route("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user = await userRepository.GetMemberAsync(username);
            if(user==null){
                return NotFound();
            }
             return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(username==null) return BadRequest("User not found in token");
            var user = await userRepository.GetUserByUsernameAsync(username);
            if(user==null) return BadRequest("User not found");
            mapper.Map(memberUpdateDTO, user);
            userRepository.Update(user);
            if(await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost,Route("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user==null) return BadRequest("Cannot Update User");
            var result = await photoService.AddPhotoAsync(file);
            if(result.Error!=null) return BadRequest(result.Error.Message);
            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count==0){
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await userRepository.SaveAllAsync()){
                return CreatedAtAction(nameof(GetUser), 
                new {username = user.UserName}, 
                mapper.Map<PhotoDTO>(photo));
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut,Route("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user==null) return BadRequest("Cannot Update User");
            var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null) return NotFound();
            if(photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
            if(currentMain!=null) currentMain.IsMain = false;
            photo.IsMain = true;
            if(await userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete, Route("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
            if(user==null) return BadRequest("Cannot Update User");
            var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo==null) return NotFound();
            if(photo.IsMain) return BadRequest("You cannot delete your main photo");
            if(photo.PublicId!=null){
                // Delete photo from Cloudinary
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }
            // Delete photo from database
            user.Photos.Remove(photo);
            if(await userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to delete photo");
        }
    }
}
