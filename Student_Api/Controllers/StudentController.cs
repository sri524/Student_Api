using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Student_Api.DTOs;
using Student_Api.Entities;
using Student_Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Student_Api.Extensions;
using Microsoft.AspNetCore.Http;
using Student_Api.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Student_Api.Controllers
{
    [Authorize]
    public class StudentController :BaseApiController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public StudentController(IStudentRepository studentRepository, IMapper mapper,
            IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _studentRepository = studentRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetStudents([FromQuery] UserPrams userPrams)
        {
            
            var students = await _studentRepository.GetStudentsAsync(userPrams);
            Response.AddPaginationHeader(students.CurrentPage, students.PageSize, students.TotalCount, students.TotalPages);
            return Ok(students);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<ProfileDto>> GetUser(string username)
        {
            var res = await _studentRepository.GetMemberAsync(username);
            return await _studentRepository.GetMemberAsync(username);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(Student_Update profileDto)
        {

            var user = await _studentRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(profileDto, user);

            _studentRepository.Update(user);

            if (await _studentRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _studentRepository.GetUserByUsernameAsync(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _studentRepository.SaveAllAsync())
            {
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
            }


            return BadRequest("Problem addding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _studentRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _studentRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _studentRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _studentRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }

        
        [HttpDelete]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _studentRepository.GetUserByUsernameAsync(User.GetUsername());
            if (student == null)
            {
                return NotFound();
            }
            _studentRepository.Delete(student);

            if (await _studentRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to Delete user");
        }

    }
}
