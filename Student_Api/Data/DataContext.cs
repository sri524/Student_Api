using Microsoft.EntityFrameworkCore;
using Student_Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Api.Data
{
    public class DataContext :DbContext
    {
        public DataContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<Student> Student { get; set; }
    }
}
