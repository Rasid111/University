using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class TeacherGroupSubjectCreateDto
    {
        public int Classroom { get; set; }

        public int TeacherProfileId { get; set; }
        public int GroupId { get; set; }
        public int SubjectId { get; set; }
    }
}
