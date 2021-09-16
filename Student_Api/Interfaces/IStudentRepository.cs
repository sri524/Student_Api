using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Student_Api.Data;
using Student_Api.DTOs;
using Student_Api.Entities;
using Student_Api.Helpers;

namespace Student_Api.Interfaces
{
    public interface IStudentRepository
    {
        void Update(Student user);
        void Delete(Student user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<Student>> GetUsersAsync();
        Task<Student> GetUserByIdAsync(int id);
        Task<Student> GetUserByUsernameAsync(string username);
        Task<PagedList<ProfileDto>> GetStudentsAsync(UserPrams userPrams);
        Task<ProfileDto> GetMemberAsync(string username);
    }
}
