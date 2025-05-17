using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class StudentProfileCreateDto
    {
        public required string UserId { get; set; }
        public int GroupId { get; set; }
    }
}
