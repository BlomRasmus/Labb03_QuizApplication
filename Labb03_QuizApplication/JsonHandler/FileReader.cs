using Labb03_QuizApplication.ViewModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Labb03_QuizApplication.JsonHandler
{
    class FileReader
    {
        public static readonly string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\packs.json";
        public static ObservableCollection<QuestionPackViewModel> JsonQuestionPack = new();
        public static async Task<ObservableCollection<QuestionPackViewModel>> ReadFile(QuestionPackViewModel newQuestionPack)
        {
            try
            {
                var myPacks = await File.ReadAllTextAsync(path);
                return JsonQuestionPack = JsonSerializer.Deserialize<ObservableCollection<QuestionPackViewModel>>(myPacks);
            }
            catch
            {
                JsonQuestionPack.Add(newQuestionPack);
                return JsonQuestionPack;
            }
        }

        public static async Task WriteFile(ObservableCollection<QuestionPackViewModel> questionPacks)
        {
            try
            {
                var myPacks = JsonSerializer.Serialize(questionPacks);
                await File.WriteAllTextAsync(path, myPacks);
            }
            catch
            {
                throw new FileLoadException("Cant save");
            }
        }
    }
}
