using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Student_Api.Data;
using Student_Api.DTOs;
using Student_Api.Entities;
using Student_Api.Helpers;
using Student_Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Api.Student_Profile
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public StudentRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

       

        public async Task<ProfileDto> GetMemberAsync(string username)
        {
            return await _context.Student
               .Where(x => x.UserName == username)
               .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
               .SingleOrDefaultAsync();
        }

        public async Task<PagedList<ProfileDto>> GetStudentsAsync(UserPrams userPrams)
        {
            var query = _context.Student.AsQueryable();
            if(userPrams.Gender!=null)
            query = query.Where(u => u.Gender == userPrams.Gender);
            if (userPrams.Grade != null)
                query = query.Where(u => u.Grade == userPrams.Grade);
            query = userPrams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<ProfileDto>.CreateAsync(query.ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(), userPrams.PageNumber, userPrams.PageSize);
        }

        public async Task<Student> GetUserByIdAsync(int id)
        {
            return await _context.Student.FindAsync(id);
        }

        public async Task<Student> GetUserByUsernameAsync(string username)
        {
            return await _context.Student
               .Include(p => p.Photos)
               .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<Student>> GetUsersAsync()
        {
            return await _context.Student
              .Include(p => p.Photos)
              .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Student student)
        {
            _context.Entry(student).State = EntityState.Modified;
        }
        public void  Delete(Student student)
        {
            _context.Student.Remove(student);
            
        }

    }
}
