using Labb03_QuizApplication.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    class AnswerViewModel
    {
        public ObjectId Id { get; set; }
        public string Answer { get; set; }
        public QuestionPackViewModel BelongingQuestionPack { get; set; }
        public Question BelongingQuestion { get; set; }
    }
}
