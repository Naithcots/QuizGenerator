using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
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
            Random random = new Random();
            connection.Open();

            string DropQuizTableString = "DROP TABLE IF EXISTS quiz";
            string DropQuestionTableString = "DROP TABLE IF EXISTS question";
            string DropAnswerTableString = "DROP TABLE IF EXISTS answer";

            string CreateQuizTableString = "CREATE TABLE IF NOT EXISTS quiz (id INTEGER PRIMARY KEY NOT NULL, title TEXT NOT NULL)";
            string CreateQuestionTableString = "CREATE TABLE IF NOT EXISTS question (id INTEGER PRIMARY KEY NOT NULL, quiz_id INT NOT NULL REFERENCES quiz(id), title TEXT NOT NULL)";
            string CreateAnswerTableString = "CREATE TABLE IF NOT EXISTS answer (id INTEGER PRIMARY KEY NOT NULL, question_id INT NOT NULL REFERENCES question(id), text TEXT NOT NULL, is_correct INT NOT NULL)";

            SqliteCommand command = connection.CreateCommand();

            // Drop tables if exists
            command.CommandText = DropAnswerTableString;
            command.ExecuteNonQuery();

            command.CommandText = DropQuestionTableString;
            command.ExecuteNonQuery();

            command.CommandText = DropQuizTableString;
            command.ExecuteNonQuery();

            // Create tables
            command.CommandText = CreateQuizTableString;
            command.ExecuteNonQuery();

            command.CommandText = CreateQuestionTableString;
            command.ExecuteNonQuery();

            command.CommandText = CreateAnswerTableString;
            command.ExecuteNonQuery();

            // Insert quiz, answers, questions data
            string InsertQuizString = @"INSERT INTO quiz(id, title) VALUES (@quizid, @quiztitle)";
            command.CommandText = InsertQuizString;
            command.Parameters.AddWithValue("@quizid", quiz.Id);
            command.Parameters.AddWithValue("@quiztitle", quiz.Name);
            command.ExecuteNonQuery();
            command.Parameters.Clear();

            foreach (QuizQuestion question in quiz.Questions)
            {
                string InsertQuestionString = @"INSERT INTO question(id, quiz_id, title) VALUES (@questionid, @quizid, @questiontitle)";
                command.CommandText = InsertQuestionString;
                command.Parameters.AddWithValue("@quizid", quiz.Id);
                command.Parameters.AddWithValue("@questionid", question.Id);
                command.Parameters.AddWithValue("@questiontitle", Base64Converter.Encode(question.Name));
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                foreach (QuizAnswer answer in question.Answers)
                {
                    string InsertAnswerString = @"INSERT INTO answer(question_id, text, is_correct) VALUES (@answerquestionid, @answertitle, @answercorrect)";
                    command.CommandText = InsertAnswerString;
                    command.Parameters.AddWithValue("@answerquestionid", question.Id);
                    command.Parameters.AddWithValue("@answertitle", Base64Converter.Encode(answer.Text));

                    if(answer.IsCorrect == 1)
                    {
                        int randomEvenInt = random.Next(1, 100) * 2;
                        string encoded = Base64Converter.Encode(randomEvenInt.ToString());
                        command.Parameters.AddWithValue("@answercorrect", encoded);
                    } else
                    {
                        int randomOddInt = random.Next(100) * 2 + 1;
                        string encoded = Base64Converter.Encode(randomOddInt.ToString());
                        command.Parameters.AddWithValue("@answercorrect", encoded);
                    }

                    //command.Parameters.AddWithValue("@answercorrect", answer.IsCorrect);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
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
                string title = Base64Converter.Decode((string)result["title"]);
                QuizQuestion question = new QuizQuestion((long)result["id"], title);
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
                string text = Base64Converter.Decode((string)result["text"]);
                int isCorrect = int.Parse(Base64Converter.Decode((string)result["is_correct"]));
                QuizAnswer answer = new QuizAnswer((long)result["id"], text, isCorrect % 2 == 0 ? 1 : 0);
                answers.Add(answer);
            }
            connection.Close();
            return answers;
        }
    }
}
