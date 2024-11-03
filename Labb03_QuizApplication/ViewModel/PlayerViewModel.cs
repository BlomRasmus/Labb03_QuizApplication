using Labb03_QuizApplication.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Labb03_QuizApplication.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private DispatcherTimer timer;

        private string _timer;

        public string Timer
        {
            get { return _timer; }
            private set 
            { 
                _timer = value;
                RaisePropertyChanged();
            }
        }

        private bool _isPlayerVisible;

        public bool IsPlayerVisible
        {
            get { return _isPlayerVisible; }
            set 
            {
                _isPlayerVisible = value;
                RaisePropertyChanged();
            }
        }


        public DelegateCommand UpdateButtonCommand { get; }

        private readonly MainWindowViewModel? mainWindowViewModel;

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            IsPlayerVisible = false;

            Timer = "Start value";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();

            UpdateButtonCommand = new DelegateCommand(UpdateButton, canUpdateButton);
        }

        private bool canUpdateButton(object? arg)
        {
            return Timer.Length < 20;
        }

        private void UpdateButton(object obj)
        {
            Timer += "x";
            UpdateButtonCommand.RaiseCanExecuteChanged();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Timer += "x";
        }

    }
}
