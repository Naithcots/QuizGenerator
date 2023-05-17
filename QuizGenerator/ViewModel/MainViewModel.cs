using Microsoft.Win32;
using QuizGenerator.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace QuizGenerator.ViewModel
{
    class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Model.DataAccess dataAccess = new Model.DataAccess();
        private Quiz quiz;

        public MainViewModel()
        {
            quiz = new Quiz();
            CurrentQuestionId = 0;
        }

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
                quiz.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuizName)));
            }
        }

        private string questionName = "";
        public string QuestionName
        {
            get => questionName;
            set
            {
                questionName = value;
                quiz.Questions[(int)currentQuestionId].Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuestionName)));
            }
        }

        private QuizAnswer[] answers;
        public QuizAnswer[] Answers
        {
            get => answers;
            set
            {
                answers = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Answers)));
            }
        }

        private ICommand createQuiz;
        public ICommand CreateQuiz
        {
            get
            {
                return createQuiz ?? new RelayCommand(
                    (o) => {
                        quiz = new Quiz();
                        QuizName = "";
                        CurrentQuestionId = 0;
                    },
                    (o) => true
                    );
            }
        }

        private ICommand createQuestion;
        public ICommand CreateQuestion
        {
            get
            {
                return createQuestion ?? new RelayCommand(
                    (o) => {
                        int newQuestionId = quiz.Questions.Count();
                        quiz.Questions.Add(new QuizQuestion(newQuestionId, ""));
                        CurrentQuestionId = newQuestionId;
                    },
                    (o) => true
                    );
            }
        }

        private ICommand deleteQuestion;
        public ICommand DeleteQuestion
        {
            get
            {
                return deleteQuestion ?? new RelayCommand(
                    (o) => {
                        quiz.Questions.RemoveAt((int)currentQuestionId);
                        CurrentQuestionId = currentQuestionId == 0 ? 0 : currentQuestionId - 1;
                    },
                    (o) => quiz.Questions.Count() > 1
                    );
            }
        }

        private ICommand prevQuestion;
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
            QuestionName = quiz.Questions[(int)CurrentQuestionId].Name;
            Answers = quiz.Questions[(int)CurrentQuestionId].Answers.ToArray();
        }


        public void SaveQuizToDatabase(string connectionString)
        {
            if (string.IsNullOrEmpty(quiz.Name) || 
                quiz.Questions.Any(e => string.IsNullOrEmpty(e.Name)) || 
                quiz.Questions.Any(q => q.Answers.Any(a => string.IsNullOrEmpty(a.Text))) ||
                // Find questions without any correct answers 
            {
                MessageBox.Show("Nie można zapisać quizu z pustymi polami.", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            dataAccess.SaveQuiz(quiz, connectionString);
            MessageBox.Show("Pomyślnie zapisano quiz.", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        public void LoadQuizFromDatabase(string connectionString)
        {
            Quiz loadedQuiz = dataAccess.ReadQuiz(connectionString);

            // Kontrola błędów nie istnieje !
            if (loadedQuiz == null) {
                MessageBox.Show("Nie można wczytać pliku.", "Błąd odczytu", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            List<QuizQuestion> questions = dataAccess.ReadQuestions(loadedQuiz.Id, connectionString);
            loadedQuiz.Questions = questions;

            // Kontrola błędów nie istnieje !
            if (questions == null) { return; }
            foreach (QuizQuestion question in questions) {
                List<QuizAnswer> answers = dataAccess.ReadAnswers(question.Id, connectionString);
                question.Answers = answers;
            }

            quiz = loadedQuiz;

            currentQuestionId = 0;
            QuizName = quiz.Name;
            QuestionName = quiz.Questions[0].Name;
            Answers = quiz.Questions[0].Answers.ToArray();
        }
    }
}
