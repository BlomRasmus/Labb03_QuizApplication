
using Labb03_QuizApplication.ViewModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace Labb03_QuizApplication.JsonHandler
{
    class FileReader
    {
        public static readonly string connectionString = "mongodb://localhost:27017/";

        public static ObservableCollection<QuestionPackViewModel> dbQuestionPack = new();
        public static ObservableCollection<CategoryViewModel> dbCategories = new();


        public static async Task<ObservableCollection<QuestionPackViewModel>> ReadFile(QuestionPackViewModel newQuestionPack)
        {
            MongoClient client = new MongoClient(connectionString);
            try
            {
                var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<QuestionPackViewModel>("QuestionPack");
                var questionPacks = await BsonQuestionPack.AsQueryable().ToListAsync();
                if (questionPacks.Count != 0)
                {
                    return dbQuestionPack = new ObservableCollection<QuestionPackViewModel>(questionPacks);
                }
                else
                {
                    dbQuestionPack.Add(newQuestionPack);
                    return dbQuestionPack;
                }
            }
            catch
            {
                dbQuestionPack.Add(newQuestionPack);
                return dbQuestionPack;
            }
        }
        public static async Task<ObservableCollection<CategoryViewModel>> LoadCategoriesAsync()
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonCategories = client.GetDatabase("RasmusBlom").GetCollection<CategoryViewModel>("Category");

            var categories = await BsonCategories.AsQueryable().ToListAsync();

            return dbCategories = new ObservableCollection<CategoryViewModel>(categories);
        }


        //public static async Task WriteFile(ObservableCollection<QuestionPackViewModel> questionPacks)
        //{
        //    MongoClient client = new MongoClient(connectionString);
        //    var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<QuestionPackViewModel>("QuestionPack");
        //    var dbPacks = await BsonQuestionPack.AsQueryable().ToListAsync();

        //    var itemsToKeep = questionPacks.Where(x => dbPacks.All(y => y.Id != x.Id));

        //    if(itemsToKeep.Count() > 0)
        //    {
        //        try
        //        {
        //            await BsonQuestionPack.InsertManyAsync(itemsToKeep);
        //        }
        //        catch
        //        {
        //            throw new FileLoadException("Cant save");
        //        }
        //    }
        //}
        public static async Task DeleteFromDb(QuestionPackViewModel packToDelete)
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<QuestionPackViewModel>("QuestionPack");

            if(BsonQuestionPack.AsQueryable().Any(p => p.Id == packToDelete.Id))
            {
                var filter = Builders<QuestionPackViewModel>.Filter.Eq(q => q.Id, packToDelete.Id);

                await BsonQuestionPack.DeleteOneAsync(filter);
            }
        }
        public static async Task UpdateDb(QuestionPackViewModel packToUpdate)
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<QuestionPackViewModel>("QuestionPack");

            var filter = Builders<QuestionPackViewModel>.Filter.Eq(q => q.Id, packToUpdate.Id);
            var update = Builders<QuestionPackViewModel>.Update
                .Set(q => q.Name, packToUpdate.Name)
                .Set(q => q.Difficulty, packToUpdate.Difficulty)
                .Set(q => q.TimeLimitInSeconds, packToUpdate.TimeLimitInSeconds)
                .Set(q => q.Questions, packToUpdate.Questions)
                .Set(q => q.Category, packToUpdate.Category);

            await BsonQuestionPack.UpdateOneAsync(filter, update);
        }
        public static async Task UpdateDb(ObservableCollection<QuestionPackViewModel> collectionToUpdate)
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<QuestionPackViewModel>("QuestionPack");

            var bulkOps = new List<WriteModel<QuestionPackViewModel>>();
            foreach (var pack in collectionToUpdate)
            {
                var filter = Builders<QuestionPackViewModel>.Filter.Eq(q => q.Id, pack.Id);
                var update = Builders<QuestionPackViewModel>.Update
                    .Set(q => q.Name, pack.Name)
                    .Set(q => q.Difficulty, pack.Difficulty)
                    .Set(q => q.TimeLimitInSeconds, pack.TimeLimitInSeconds)
                    .Set(q => q.Questions, pack.Questions)
                    .Set(q => q.Category, pack.Category);

                var updateOne = new UpdateOneModel<QuestionPackViewModel>(filter, update) { IsUpsert = true };
                bulkOps.Add(updateOne);
            }

            if (bulkOps.Count > 0)
            {
                await BsonQuestionPack.BulkWriteAsync(bulkOps);
            }
        }
        public static async Task AddCategoryToDb(CategoryViewModel category)
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<CategoryViewModel>("Category");

            if(category != null)
            {
                await BsonQuestionPack.InsertOneAsync(category);
            }
        }
        public static async Task RemoveCategoryFromDb(CategoryViewModel category)
        {
            MongoClient client = new MongoClient(connectionString);
            var BsonQuestionPack = client.GetDatabase("RasmusBlom").GetCollection<CategoryViewModel>("Category");

            var filter = Builders<CategoryViewModel>.Filter.Eq(c => c.Id, category.Id);

            await BsonQuestionPack.DeleteOneAsync(filter);
        }
    }
}