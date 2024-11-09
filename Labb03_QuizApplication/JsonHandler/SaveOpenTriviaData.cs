using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.JsonHandler
{

    public class Rootobject
    {
        public int response_code { get; set; }
        public Result[] results { get; set; }
    }

    public class Result
    {
        public string type { get; set; }
        public string difficulty { get; set; }
        public string category { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public string[] incorrect_answers { get; set; }
    }


    public class RootobjectCategories
    {
        public Trivia_Categories[] trivia_categories { get; set; }
    }

    public class Trivia_Categories
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}
