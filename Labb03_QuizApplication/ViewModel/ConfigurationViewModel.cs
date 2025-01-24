using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.DialogServices;
using Labb03_QuizApplication.JsonHandler;
using Labb03_QuizApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.ViewModel
{
    class ConfigurationViewModel : ViewModelBase
    {
        IDialog dialog = new DialogService();

        private Question? _selectedQuestion;

        public Question? SelectedQuestion
        {
            get { return _selectedQuestion; }
            set 
            { 
                _selectedQuestion = value;
                Visibility = SelectedQuestion != null;
                RemoveItemCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        private bool _isConfigVisible;

        public bool IsConfigVisible
        {
            get { return _isConfigVisible; }
            set 
            {
                _isConfigVisible = value;
                RaisePropertyChanged();
            }
        }

        private bool _visibility;

        public bool Visibility
        {
            get { return _visibility; }
            set 
            {
                _visibility = value;
                AddItemCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        public DelegateCommand AddItemCommand { get; }

        public DelegateCommand RemoveItemCommand { get; }

        public DelegateCommand ShowDialogCommand => new DelegateCommand(myDialog => ShowDialog());
        public DelegateCommand UpdatePackOptionsCommand { get; }


        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel?.ActivePack; }

        private readonly MainWindowViewModel? mainWindowViewModel;

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            RemoveItemCommand = new DelegateCommand(RemoveItem, canExecute => SelectedQuestion != null);
            IsConfigVisible = true;
            AddItemCommand = new DelegateCommand(CreateQuestion);
            UpdatePackOptionsCommand = new DelegateCommand(UpdatePackOptions);
        }


        public void CreateQuestion(object parameter)
        {
            var question = new Question("", "", "", "", "");
            ActivePack.Questions.Add(question);
            SelectedQuestion = question;
            RaisePropertyChanged("ActivePack");
            mainWindowViewModel.SetPlayerVisCommand.RaiseCanExecuteChanged();

        }

        public void RemoveItem(object parameter)
        {
            ActivePack.Questions.Remove(SelectedQuestion);
            RaisePropertyChanged("ActivePack");

            if(ActivePack.Questions.Any()) mainWindowViewModel.SetPlayerVisCommand.RaiseCanExecuteChanged();
            FileReader.UpdateDbAsync(ActivePack);
        }

        public void ShowDialog()
        {
            dialog.ShowPackOptionsDialog();
        }
        public void UpdatePackOptions(object obj)
        {
            FileReader.UpdateDbAsync(ActivePack);
        }

    }
}
