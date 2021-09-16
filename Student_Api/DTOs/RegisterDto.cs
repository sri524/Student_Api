using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Api.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
       [Required] public string FirstName { get; set; }
       [Required] public string MiddleName { get; set; }
       [Required] public string LastName { get; set; }
       [Required] public string Gender { get; set; }
       [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string Grade { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }

    }
}
