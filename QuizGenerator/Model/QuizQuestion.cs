using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Model
{
    class QuizQuestion
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();

        public QuizQuestion(long id, string name)
        {
            this.Id = id;
            this.Name = name;
            List<QuizAnswer> answers = new List<QuizAnswer>();
            for (int i = 0; i < 4; i++)
            {
                answers.Add(new QuizAnswer("", 0));
            }
            this.Answers = answers;
        }

        public QuizQuestion(string name, List<QuizAnswer> answers)
        {
            this.Name = name;
            this.Answers = answers;
        }
    }
}
