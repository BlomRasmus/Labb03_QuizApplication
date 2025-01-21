using Labb03_QuizApplication.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    class QuestionPackViewModel : ViewModelBase
    {

        private readonly QuestionPack model;


        public QuestionPackViewModel(QuestionPack Model)
        {
            this.model = Model;
            this.Questions = new ObservableCollection<Question>(model.Questions);
        }

        [BsonConstructor]
        public QuestionPackViewModel()
        {
            model = new QuestionPack() { Questions = [] };
            Questions = new ObservableCollection<Question>(model.Questions);
        }

        public ObservableCollection<Question> Questions { get; set; }

        public ObjectId Id { get; set; }
        public string Name 
        {
            get => model.Name;
            set
            {
                model.Name = value;
                RaisePropertyChanged();
            }
        }

        public Difficulty Difficulty
        {
            get => model.Difficulty;
            set
            {
                model.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        public int TimeLimitInSeconds
        {
            get => model.TimeLimitInSeconds;
            set
            {
                model.TimeLimitInSeconds = value;
                RaisePropertyChanged();
            }
        }

        private CategoryViewModel _category;

        public CategoryViewModel Category
        {
            get { return _category; }
            set { _category = value; RaisePropertyChanged(); }
        }



        public override string ToString()
        {
            return $"{Name}   ({Difficulty})";
        }

    }
}
