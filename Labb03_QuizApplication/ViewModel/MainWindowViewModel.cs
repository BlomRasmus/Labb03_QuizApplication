using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.DialogServices;
using Labb03_QuizApplication.JsonHandler;
using Labb03_QuizApplication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Labb03_QuizApplication.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
		//TODO: Lägg i Egen klass
		//private readonly string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\packs.json";

        DialogService AddPackDialog = new DialogService();
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }
        public PlayerViewModel PlayerViewModel { get; }
        public ConfigurationViewModel ConfigurationViewModel { get; }


        public DelegateCommand AddQuestionPackCommand { get; }
        public DelegateCommand SetNewActivePackCommand { get; }
		public DelegateCommand SetPlayerVisCommand { get; }
        public DelegateCommand SetConfigVisCommand { get; }
		public DelegateCommand SetActivePackCommand { get; }
		public DelegateCommand DeleteQuestionPackCommand { get; }
		public DelegateCommand EditNewQuestionPackCommand { get; }
		public DelegateCommand ExitWindowCommand { get; }



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
				ConfigurationViewModel?.RaisePropertyChanged();
			}
		}

        public MainWindowViewModel()
        {
            Packs = new();
			ConfigurationViewModel = new ConfigurationViewModel(this);
			ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
			PlayerViewModel = new PlayerViewModel(this);
			LoadData(ActivePack);
			//Använd activePack för att binda allt till, programmet kommer enbart använda den aktiva questionPack, och den kommer uppdateras beroende på vilken som används

			AddQuestionPackCommand = new DelegateCommand(AddQuestionPack);
			SetConfigVisCommand = new DelegateCommand(SetConfigVis, canChange => ConfigurationViewModel.IsConfigVisible != true);
			SetPlayerVisCommand = new DelegateCommand(SetPlayerVis, canChange => ActivePack.Questions.Any() && PlayerViewModel.IsPlayerVisible != true);
			SetActivePackCommand = new DelegateCommand(SetActivePack);
			DeleteQuestionPackCommand = new DelegateCommand(DeleteQuestionPack, IsNotEmpty => Packs.Count > 1);
			EditNewQuestionPackCommand = new DelegateCommand(EditNewQuestionPack);
			ExitWindowCommand = new DelegateCommand(ExitWindowAsync);
        }

		public async Task LoadData(QuestionPackViewModel newQuestionPack)
		{
			Packs = await FileReader.ReadFile(newQuestionPack);
			ActivePack = Packs.FirstOrDefault();
		}

		public async Task SaveData(ObservableCollection<QuestionPackViewModel> packs)
		{
			await FileReader.WriteFile(packs);
		}
		public void EditNewQuestionPack(object parameter)
		{
            QuestionPack questionPack = new QuestionPack("My Question Pack");
            NewQuestionPack = new QuestionPackViewModel(questionPack);
            AddPackDialog.ShowCreatePackModelDialog();
        }
        public void AddQuestionPack(object parameter)
		{
			Packs.Add(NewQuestionPack);
			DeleteQuestionPackCommand.RaiseCanExecuteChanged();
            ActivePack = NewQuestionPack;
            RaisePropertyChanged();
		}
		public void DeleteQuestionPack(object parameter)
		{
			Packs.Remove(ActivePack);
			RaisePropertyChanged("Packs");
            DeleteQuestionPackCommand.RaiseCanExecuteChanged();
            ActivePack = Packs.FirstOrDefault();
		}

		public void SetPlayerVis(object parameter)
		{
			PlayerViewModel.IsPlayerVisible = true;
			ConfigurationViewModel.IsConfigVisible = false;
            SetConfigVisCommand.RaiseCanExecuteChanged();
            SetPlayerVisCommand.RaiseCanExecuteChanged();
        }
        public async void SetConfigVis(object parameter)
        {
            PlayerViewModel.IsPlayerVisible = false;
            ConfigurationViewModel.IsConfigVisible = true;
            SetConfigVisCommand.RaiseCanExecuteChanged();
            SetPlayerVisCommand.RaiseCanExecuteChanged();
        }
		public async void ExitWindowAsync(object? obj)
		{
            await SaveData(Packs);
			Application.Current.Shutdown();
		}

		public void SetActivePack(object obj) => ActivePack = obj as QuestionPackViewModel;

    }
}
