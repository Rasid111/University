using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class TestCreateDto
    {
        public required string Title { get; set; }
        public int SubjectId { get; set; }
        public List<QuestionCreateDto>? Questions { get; set; } = [];
    }
}
