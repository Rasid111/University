using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class GroupCreateDto
    {
        public required string Name { get; set; }
        public int Year { get; set; } = 1;
        public int FacultyId { get; set; }
        public int MajorId { get; set; }
    }
}
