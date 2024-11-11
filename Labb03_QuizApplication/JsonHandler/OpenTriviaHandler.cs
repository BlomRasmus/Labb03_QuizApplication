using Labb03_QuizApplication.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Labb03_QuizApplication.JsonHandler
{
    internal class OpenTriviaHandler
    {

        //Jag försökte iaf :)

        HttpClient client = new HttpClient();

        public async Task<Rootobject> GetQuestions(int amount, int id, string difficulty = "medium")
        {

            Uri baseuri = new Uri("https://opentdb.com");
            string modifiedUri = $"/api.php?amount={amount}&category={id}&difficulty={difficulty}&type=multiple";

            Uri fullUri = new Uri(baseuri, modifiedUri);
            Rootobject myQuestion = new Rootobject();

            try
            {
                string ApiData = await client.GetStringAsync(fullUri);
                myQuestion = JsonSerializer.Deserialize<Rootobject>(ApiData);

                foreach(var questions in myQuestion.results)
                {
                    questions.question = HttpUtility.HtmlDecode(questions.question);
                    questions.correct_answer = HttpUtility.HtmlDecode(questions.correct_answer);

                    for ( int i = 0; i < questions.incorrect_answers.Length; i++)
                    {
                        questions.incorrect_answers[i] = HttpUtility.HtmlDecode(questions.incorrect_answers[i]);
                    }

                }

                return myQuestion;
            }
            catch(Exception e)
            {
                if(e.Message == "Response status code does not indicate success: 429 (Too Many Requests).")
                {
                    myQuestion.response_code = 5;
                }
                else if(e.Message == "No such host is known. (opentdb.com:443)")
                {
                    myQuestion.response_code = 6;
                }
                else
                {
                    myQuestion.response_code = 7;
                }

                return myQuestion;
            }
            
        }

        public async Task<List<Trivia_Categories>> GetCategories()
        {

            string categoriesJson = await client.GetStringAsync("https://opentdb.com/api_category.php");
            RootobjectCategories categories = JsonSerializer.Deserialize<RootobjectCategories>(categoriesJson);
            return categories.trivia_categories.ToList();

        }

        public string ShowImportStatus(int responseCode)
        {


            switch (responseCode)
            {
                case 0:
                    return "Success! Returned results successfully.";
                case 1:
                    return "No Results Could not return results. The API doesn't have enough questions for your query.";
                case 2:
                    return "Invalid Parameter Contains an invalid parameter. Arguements passed in aren't valid.";
                case 3:
                    return "Token Not Found Session Token does not exist.";
                case 4:
                    return "Token Empty Session Token has returned all possible questions for the specified query.Resetting the Token is necessary.";
                case 5:
                    return "Rate Limit Too many requests have occurred. Each IP can only access the API once every 5 seconds.";
                case 6:
                    return "No such host is known. (opentdb.com:443)";
                default:
                    return "Unexpected error occured";
            }
        }

    }
}
