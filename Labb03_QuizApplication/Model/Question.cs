using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.Model
{
    public class Question
    {
        public Question(string query, string correctAnswer,
            string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;

            IncorrectAnswers = new string[3] { incorrectAnswer1, incorrectAnswer2, incorrectAnswer3 }; 
        }

        [JsonConstructor]
        public Question()
        {
            Query = string.Empty;
            CorrectAnswer = string.Empty;
            IncorrectAnswers = [];
        }

        public string Query { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] IncorrectAnswers { get; set; }
    }
}
