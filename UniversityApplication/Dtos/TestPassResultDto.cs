using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApplication.Dtos
{
    public class TestPassResultDto
    {
        public required Dictionary<string, string> QuestionCorrectAnswers { get; set; }
        public required List<string> StudentAnswers { get; set; }
        public decimal Result;
    }
}
