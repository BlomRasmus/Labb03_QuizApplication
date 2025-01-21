using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    public class CategoryViewModel
    {
        public ObjectId Id { get; set; }

        public string Category { get; set; }

        public override string ToString()
        {
            return $"{Category}";
        }
    }
}
