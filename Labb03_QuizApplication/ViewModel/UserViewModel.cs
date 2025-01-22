using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    class UserViewModel
    {

        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public QuestionPackViewModel UserPack { get; set; }
    }
}
