using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace UniversityApplication.Dtos
{
    public class QuestionCreateDto
    {
        public required string Title { get; set; }
        public required string CorrectAnswerTitle { get; set; }
        public int TestId { get; set; }
        public List<QuestionAnswerCreateDto>? Answers { get; set; } = [];
    }
}
