using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class QuestionAnswerCreateDto
    {
        public required string Title { get; set; }
        public int QuestionId { get; set; }
    }
}
