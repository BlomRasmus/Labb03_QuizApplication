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
		public OpenTriviaHandler test;


        DialogService ShowDialog = new DialogService();
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }
		public ObservableCollection<Trivia_Categories> Categories { get; set; }
		public ObservableCollection<string> Difficulties { get; set; } = new ObservableCollection<string> { "easy", "medium", "hard" };
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
		public DelegateCommand ShowImportQuestionsDialogCommand { get; }
		public DelegateCommand ImportQuestionsCommand { get; }

		private string _statusReport;

		public string StatusReport
		{
			get { return _statusReport; }
			set 
			{
				_statusReport = value;
				RaisePropertyChanged();
			}
		}


		private int _categoryIndex;

		public int CategoryIndex
		{
			get { return _categoryIndex; }
			set 
			{
				_categoryIndex = value;
				RaisePropertyChanged();
			}
		}

		private string _importQuestionDifficulty;

		public string ImportQuestionDifficulty
		{
			get { return _importQuestionDifficulty; }
			set 
			{
				_importQuestionDifficulty = value;
				RaisePropertyChanged();
			}
		}

		private int _numberOfImportedQuestions;

		public int NumberOfImportedQuestions
		{
			get { return _numberOfImportedQuestions; }
			set 
			{
				_numberOfImportedQuestions = value;
				RaisePropertyChanged();
			}
		}




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
            test = new();
            Packs = new();
			ConfigurationViewModel = new ConfigurationViewModel(this);
			ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
			PlayerViewModel = new PlayerViewModel(this);


			LoadData(ActivePack);
			SetCategoryList();
			

			AddQuestionPackCommand = new DelegateCommand(AddQuestionPack);
			SetConfigVisCommand = new DelegateCommand(SetConfigVis, canChange => ConfigurationViewModel.IsConfigVisible != true);
			SetPlayerVisCommand = new DelegateCommand(SetPlayerVis, canChange => ActivePack.Questions.Any() && PlayerViewModel.IsPlayerVisible != true);
			SetActivePackCommand = new DelegateCommand(SetActivePack);
			DeleteQuestionPackCommand = new DelegateCommand(DeleteQuestionPack, IsNotEmpty => Packs.Count > 1);
			EditNewQuestionPackCommand = new DelegateCommand(EditNewQuestionPack);
			ExitWindowCommand = new DelegateCommand(ExitWindowAsync);
			ShowImportQuestionsDialogCommand = new DelegateCommand(ShowImportDialog);
			ImportQuestionsCommand = new DelegateCommand(GetImportedData);
        }


		public async void GetImportedData(object obj)
		{
			await ImportData();
		}
		public async Task SetCategoryList()
		{
			Categories = new ObservableCollection<Trivia_Categories>(await test.GetCategories());
        }

		public async Task LoadData(QuestionPackViewModel newQuestionPack)
		{
			Packs = await FileReader.ReadFile(newQuestionPack);

			if(Packs != null)
			{
				ActivePack = Packs.FirstOrDefault();
			}
			else
			{
				ActivePack = new QuestionPackViewModel();
			}
		}

		public async Task SaveData(ObservableCollection<QuestionPackViewModel> packs)
		{
			await FileReader.WriteFile(packs);
		}

		public async Task ImportData()
		{
			int id = Categories[CategoryIndex].id;

            OpenTriviaHandler test = new();


			Rootobject importedData = await test.GetQuestions(NumberOfImportedQuestions, id, ImportQuestionDifficulty);


			StatusReport = test.ShowImportStatus(importedData.response_code);


			if(importedData.results != null)
			{
				foreach (var item in importedData.results)
				{
					ActivePack.Questions
						.Add(new Question(item.question,
						item.correct_answer,
						item.incorrect_answers[0],
						item.incorrect_answers[1],
						item.incorrect_answers[2]));

				}
			}
        }
		public void EditNewQuestionPack(object parameter)
		{
            QuestionPack questionPack = new QuestionPack("My Question Pack");
            NewQuestionPack = new QuestionPackViewModel(questionPack);
            ShowDialog.ShowCreatePackModelDialog();
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


		public void ShowImportDialog(object parameter)
		{
			ShowDialog.ShowImportQuestionsDialog();
		}

		public void SetActivePack(object obj) => ActivePack = obj as QuestionPackViewModel;

    }
}
