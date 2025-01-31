﻿using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.DialogServices;
using Labb03_QuizApplication.JsonHandler;
using Labb03_QuizApplication.Model;
using MongoDB.Bson;
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
using System.Windows.Forms;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Labb03_QuizApplication.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
		public OpenTriviaHandler TriviaHandler;


        DialogService ShowDialog = new DialogService();
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }
		public ObservableCollection<Trivia_Categories> Categories { get; set; }
		public ObservableCollection<string> Difficulties { get; set; } = new ObservableCollection<string> { "easy", "medium", "hard" };
		public ObservableCollection<CategoryViewModel> QuestionPackCategories { get; set; }
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
		public DelegateCommand ShowAddCategoryDialogCommand { get; }
        public DelegateCommand AddCategoryCommand { get; }
		public DelegateCommand RemoveCategoryCommand { get; }
		public DelegateCommand ShowRemoveCategoryDialogCommand { get; }


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

		private CategoryViewModel _newCategory;

		public CategoryViewModel SelectedCategory
		{
			get { return _newCategory; }
			set 
			{
				_newCategory = value; 
				RaisePropertyChanged(); 
				ShowRemoveCategoryDialogCommand.RaiseCanExecuteChanged(); 
			}
		}


		public MainWindowViewModel()
        {
            TriviaHandler = new();
            Packs = new();
			QuestionPackCategories = new();
			ConfigurationViewModel = new ConfigurationViewModel(this);
			ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
			PlayerViewModel = new PlayerViewModel(this);
			NumberOfImportedQuestions = 1;

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
			ShowAddCategoryDialogCommand = new DelegateCommand(ShowAddCategoryDialog);
			AddCategoryCommand = new DelegateCommand(AddCategory);
			RemoveCategoryCommand = new DelegateCommand(RemoveCategory, IsNotNull => SelectedCategory != null);
			ShowRemoveCategoryDialogCommand = new DelegateCommand(ShowRemoveCategoryDialog, isNotEmpty => QuestionPackCategories.Count > 0);
        }


		public async void GetImportedData(object obj)
		{
			try
			{
				await ImportData();
			}
			catch
			{
				StatusReport = TriviaHandler.ShowImportStatus(6);
                ShowDialog.ShowImportStatusDialog();
            }
			
		}
		public async Task SetCategoryList()
		{
			Categories = new ObservableCollection<Trivia_Categories>(await TriviaHandler.GetCategories());
        }

		public async Task LoadData(QuestionPackViewModel newQuestionPack)
		{
			Packs = await FileReader.GetQuestionPacksFromDbAsync(newQuestionPack);
			QuestionPackCategories = await FileReader.LoadCategoriesAsync();
			RaisePropertyChanged("Packs");

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
			await FileReader.UpdateDbAsync(packs);
		}

		public async Task ImportData()
		{

            OpenTriviaHandler TriviaHandler = new();


			int id = Categories[CategoryIndex].id;

			Rootobject importedData = await TriviaHandler.GetQuestions(NumberOfImportedQuestions, id, ImportQuestionDifficulty);




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
				StatusReport = TriviaHandler.ShowImportStatus(importedData.response_code);
			}
			else
			{
                StatusReport = TriviaHandler.ShowImportStatus(importedData.response_code);
            }

			SetConfigVis(null);
			ShowDialog.ShowImportStatusDialog();
        }
		public void EditNewQuestionPack(object parameter)
		{
            QuestionPack questionPack = new QuestionPack("My Question Pack");
			NewQuestionPack = new QuestionPackViewModel(questionPack) { Id = ObjectId.GenerateNewId() };
            ShowDialog.ShowCreatePackModelDialog();
        }
        public void AddQuestionPack(object parameter)
		{
			Packs.Add(NewQuestionPack);
			DeleteQuestionPackCommand.RaiseCanExecuteChanged();
            ActivePack = NewQuestionPack;
			SetConfigVis(null);
            RaisePropertyChanged();
		}
		public void DeleteQuestionPack(object parameter)
		{
			Packs.Remove(ActivePack);
			FileReader.DeleteFromDbAsync(ActivePack);
			RaisePropertyChanged("Packs");
            DeleteQuestionPackCommand.RaiseCanExecuteChanged();
            ActivePack = Packs.FirstOrDefault();
			SetConfigVis(null);
		}

		public void SetPlayerVis(object parameter)
		{
			PlayerViewModel.IsBeginningOfQuiz = true;
			ConfigurationViewModel.IsConfigVisible = false;
            SetConfigVisCommand.RaiseCanExecuteChanged();
            SetPlayerVisCommand.RaiseCanExecuteChanged();
        }
        public void SetConfigVis(object? parameter)
        {
			if (PlayerViewModel.IsPlayerVisible == true)
			{
				PlayerViewModel.IsPlayerVisible = false;
                PlayerViewModel.IsEndOfQuiz = false;
            }
			else if (PlayerViewModel.IsBeginningOfQuiz == true)
			{
				PlayerViewModel.IsBeginningOfQuiz = false;
			}
			else if (PlayerViewModel.IsEndOfQuiz == true)
			{
				PlayerViewModel.IsEndOfQuiz = false;
			}

            ConfigurationViewModel.IsConfigVisible = true;
            SetConfigVisCommand.RaiseCanExecuteChanged();
            SetPlayerVisCommand.RaiseCanExecuteChanged();
        }

		public void AddCategory(object parameter)
		{
			QuestionPackCategories.Add(SelectedCategory);
			FileReader.AddCategoryToDbAsync(SelectedCategory);
		}
		public void RemoveCategory(object parameter)
		{
			FileReader.RemoveCategoryFromDbAsync(SelectedCategory);
			QuestionPackCategories.Remove(SelectedCategory);
		}
		public async void ExitWindowAsync(object? obj)
		{
            await SaveData(Packs);
            System.Windows.Application.Current.Shutdown();
		}


		public void ShowImportDialog(object parameter)
		{
			ShowDialog.ShowImportQuestionsDialog();
		}
		public void ShowAddCategoryDialog(object parameter)
		{
			SelectedCategory = new CategoryViewModel();
			ShowDialog.ShowAddCategoryDialog();
		}
		public void ShowRemoveCategoryDialog(object Parameter)
		{
			SelectedCategory = QuestionPackCategories.FirstOrDefault();
			ShowDialog.ShowRemoveCategoryDialog();
		}

		public void SetActivePack(object obj)
		{
			ActivePack = obj as QuestionPackViewModel;
			SetConfigVis(null);
		}
    }
}
