using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGenerator.Model
{
    class DataAccess
    {
        //SqliteConnection connection = new SqliteConnection(@"Data Source=C:\quiz1.db");

        public void SaveQuiz(Quiz quiz, string connectionString)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={connectionString}");
            connection.Open();

            string CreateQuizTableString = "CREATE TABLE IF NOT EXISTS quiz (id INTEGER PRIMARY KEY NOT NULL, title TEXT NOT NULL)";
            string CreateQuestionTableString = "CREATE TABLE IF NOT EXISTS question (id INTEGER PRIMARY KEY NOT NULL, quiz_id INT NOT NULL REFERENCES quiz(id), title TEXT NOT NULL)";
            string CreateAnswerTableString = "CREATE TABLE IF NOT EXISTS answer (id INTEGER PRIMARY KEY NOT NULL, question_id INT NOT NULL REFERENCES question(id), text TEXT NOT NULL, is_correct INT NOT NULL)";

            SqliteCommand command = connection.CreateCommand();

            // Create tables
            command.CommandText = CreateQuizTableString;
            command.ExecuteNonQuery();

            command.CommandText = CreateQuestionTableString;
            command.ExecuteNonQuery();

            command.CommandText = CreateAnswerTableString;
            command.ExecuteNonQuery();

            // Insert quiz, answers, questions data
            string InsertQuizString = "INSERT INTO quiz(id, title) VALUES (@quizid, @quiztitle)";
            command.CommandText = InsertQuizString;
            command.Parameters.Add(new SqliteParameter("@quizid", quiz.Id));
            command.Parameters.Add(new SqliteParameter("@quiztitle", quiz.Name));
            command.ExecuteNonQuery();

            foreach (QuizQuestion question in quiz.Questions)
            {
                string InsertQuestionString = "INSERT INTO question(quiz_id, title) VALUES (@quizid, @questiontitle)";
                command.CommandText = InsertQuestionString;
                command.Parameters.Add(new SqliteParameter("@questiontitle", question.Name));
                command.ExecuteNonQuery();

                foreach (QuizAnswer answer in question.Answers)
                {
                    string InsertAnswerString = "INSERT INTO answer(question_id, text, is_correct) VALUES (@answerquestionid, @answertitle, @answercorrect)";
                    command.CommandText = InsertAnswerString;
                    command.Parameters.Add(new SqliteParameter("@answerquestionid", question.Id));
                    command.Parameters.Add(new SqliteParameter("@answertitle", answer.Text));
                    command.Parameters.Add(new SqliteParameter("@answercorrect", answer.IsCorrect));
                    command.ExecuteNonQuery();
                }
            }
            connection.Close();
        }

        public Quiz? ReadQuiz(string connectionString)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={connectionString}");

            connection.Open();
            string getQuizQuery = "SELECT * FROM quiz LIMIT 1";
            var command = new SqliteCommand(getQuizQuery, connection );
            SqliteDataReader result = command.ExecuteReader();
            while (result.Read())
            {
                Quiz quiz = new Quiz((long)result["id"], (string)result["title"]);
                connection.Close();
                return quiz;
            }
            connection.Close();
            return null;
        }

        public List<QuizQuestion>? ReadQuestions(long quizId, string connectionString)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={connectionString}");
            List<QuizQuestion> questions = new List<QuizQuestion> {};

            connection.Open();
            string getQuizQuery = "SELECT * FROM question WHERE quiz_id=$1";
            var command = new SqliteCommand(getQuizQuery, connection);
            command.Parameters.Add(new SqliteParameter("$1", quizId));
            SqliteDataReader result = command.ExecuteReader();
            while (result.Read())
            {
                QuizQuestion question = new QuizQuestion((long)result["id"], (string)result["title"]);
                questions.Add(question);
            }
            connection.Close();
            return questions;
        }

        public List<QuizAnswer>? ReadAnswers(long questionId, string connectionString)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={connectionString}");
            List<QuizAnswer> answers = new List<QuizAnswer> { };

            connection.Open();
            string getQuizQuery = "SELECT * FROM answer WHERE question_id=$1";
            var command = new SqliteCommand(getQuizQuery, connection);
            command.Parameters.Add(new SqliteParameter("$1", questionId));
            SqliteDataReader result = command.ExecuteReader();
            while (result.Read())
            {
                QuizAnswer answer = new QuizAnswer((long)result["id"], (string)result["text"], (long)result["is_correct"]);
                answers.Add(answer);
            }
            connection.Close();
            return answers;
        }
    }
}
