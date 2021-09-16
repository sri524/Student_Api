using Student_Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Student student);
    }
}
