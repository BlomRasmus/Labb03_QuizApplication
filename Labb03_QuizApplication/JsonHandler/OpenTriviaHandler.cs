using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.JsonHandler
{
    internal class OpenTriviaHandler
    {

        //Jag vet att den inte är perfekt, men jag gjorde mitt bästa! :)

        HttpClient client = new HttpClient();

        public async Task<Rootobject> GetQuestions(int amount, int id, string difficulty = "medium")
        {

            Uri baseuri = new Uri("https://opentdb.com");
            string modifiedUri = $"/api.php?amount={amount}&category={id}&difficulty={difficulty}&type=multiple";

            Uri fullUri = new Uri(baseuri, modifiedUri);

            string testString = await client.GetStringAsync(fullUri);
            Rootobject myQuestions = JsonSerializer.Deserialize<Rootobject>(testString);
            return myQuestions;
        }

        public async Task<List<Trivia_Categories>> GetCategories()
        {
            string categoriesJson = await client.GetStringAsync("https://opentdb.com/api_category.php");
            RootobjectCategories categories = JsonSerializer.Deserialize<RootobjectCategories>(categoriesJson);
            return categories.trivia_categories.ToList();
        }

    }
}
