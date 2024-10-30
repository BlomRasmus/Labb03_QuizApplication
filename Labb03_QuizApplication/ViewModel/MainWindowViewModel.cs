using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.DialogServices;
using Labb03_QuizApplication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
		DialogService AddPackDialog = new DialogService();
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

        public PlayerViewModel PlayerViewModel { get; }

        public ConfigurationViewModel ConfigurationViewModel { get; }


        public DelegateCommand AddQuestionPackCommand { get; }
        public DelegateCommand SetActivePackCommand { get; }


        private QuestionPackViewModel _newQuestionPack;
        public QuestionPackViewModel NewQuestionPack 
		{ 
			get => _newQuestionPack;
			set
			{
				_newQuestionPack = value;
				RaisePropertyChanged();
			}
		}

        private QuestionPackViewModel? _activePack;
        public QuestionPackViewModel? ActivePack
		{
			get => _activePack; 
			set 
			{
				_activePack = value;
				RaisePropertyChanged();
				ConfigurationViewModel.RaisePropertyChanged("ActivePack");
			}
		}

        public MainWindowViewModel()
        {
			ConfigurationViewModel = new ConfigurationViewModel(this);
			PlayerViewModel = new PlayerViewModel(this);
			Packs = new();
			ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
			Packs.Add(ActivePack);
			//Använd activePack för att binda allt till, programmet kommer enbart använda den aktiva questionPack, och den kommer uppdateras beroende på vilken som används

			AddQuestionPackCommand = new DelegateCommand(AddQuestionPack);
            SetActivePackCommand = new DelegateCommand(SetActivePack);
        }

        public void AddQuestionPack(object parameter)
		{
			QuestionPack questionPack = new QuestionPack("My Question Pack");
			NewQuestionPack = new QuestionPackViewModel(questionPack);
			Packs.Add(NewQuestionPack);
			AddPackDialog.ShowCreatePackModelDialog();
			RaisePropertyChanged();
		}
		public void SetActivePack(object parameter)
		{
			ActivePack = NewQuestionPack;
			RaisePropertyChanged();
		}


    }
}
