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
