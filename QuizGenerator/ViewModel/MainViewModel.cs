using Microsoft.Win32;
using QuizGenerator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuizGenerator.ViewModel
{
    class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Model.DataAccess dataAccess = new Model.DataAccess();
        private Quiz quiz;

        private long currentQuestionId = 0;
        public long CurrentQuestionId
        {
            get => currentQuestionId;
            set { currentQuestionId = value; HandleQuestionChange(); }
        }



        private string quizName = "";
        public string QuizName
        {
            get => quizName;
            set
            {
                quizName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(QuizName)));
            }
        }

        private string questionName = "";
        public string QuestionName
        {
            get => questionName;
            set
            {
                questionName = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionName)));
            }
        }

        private QuizAnswer[] answers;
        public QuizAnswer[] Answers
        {
            get => answers;
            set
            {
                answers = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Answers)));
            }
        }

        public ICommand prevQuestion;
        public ICommand PrevQuestion
        {
            get
            {
                return prevQuestion ?? new RelayCommand(
                    (o => { CurrentQuestionId--; }),
                    (o) => { return quiz != null && CurrentQuestionId > 0; });
            }
        }

        public ICommand nextQuestion;
        public ICommand NextQuestion
        {
            get
            {
                return nextQuestion ?? new RelayCommand(
                    (o => { CurrentQuestionId++; }), 
                    (o) => { return quiz != null && CurrentQuestionId < quiz.Questions.Count - 1; });
            }
        }

        public void HandleQuestionChange()
        {
            QuestionName = quiz.Questions[(int)currentQuestionId].Name;
            Answers = quiz.Questions[(int)currentQuestionId].Answers.ToArray();
        }


        public void LoadQuizFromDatabase(string connectionString)
        {
            Quiz loadedQuiz = dataAccess.ReadQuiz(connectionString);

            // Kontrola błędów !
            if (loadedQuiz == null) { return; }

            List<QuizQuestion> questions = dataAccess.ReadQuestions(loadedQuiz.Id, connectionString);
            loadedQuiz.Questions = questions;

            // Kontrola błędów !
            if (questions == null) { return; }
            foreach (QuizQuestion question in questions) {
                List<QuizAnswer> answers = dataAccess.ReadAnswers(question.Id, connectionString);
                question.Answers = answers;
            }

            quiz = loadedQuiz;

            QuizName = quiz.Name;
            QuestionName = quiz.Questions[0].Name;
            Answers = quiz.Questions[0].Answers.ToArray();
        }
    }
}
