using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Model
{
    class Quiz
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

        public Quiz()
        {
            this.Id = 1;
            this.Questions.Add(new QuizQuestion(0, ""));
            List<QuizAnswer> answers = new List<QuizAnswer>();
            for (int i = 0; i < 4; i++)
            {
                answers.Add(new QuizAnswer(i, "", 0));
            }
            this.Questions[0].Answers = answers;
        }

        public Quiz(long id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Questions = new List<QuizQuestion>();
        }

        public Quiz(long id, string name, List<QuizQuestion> questions) { 
            this.Id = id;
            this.Name = name;
            this.Questions = questions;
        }
    }
}
