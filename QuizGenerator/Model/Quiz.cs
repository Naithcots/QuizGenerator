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
        public List<QuizQuestion> Questions { get; set; }

        public Quiz(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Quiz(long id, string name, List<QuizQuestion> questions) { 
            this.Id = id;
            this.Name = name;
            this.Questions = questions;
        }
    }
}
