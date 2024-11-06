using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Labb03_QuizApplication.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {
        private int currentQuestionIndex;
        private int correctAnswers = 0;



        private bool _makeWayForEndMessage;

        public bool MakeWayForEndMessage
        {
            get { return _makeWayForEndMessage; }
            set 
            {
                _makeWayForEndMessage = value;
                RaisePropertyChanged();
            }
        }


        private bool _isEndOfQuiz;

        public bool IsEndOfQuiz
        {
            get { return _isEndOfQuiz; }
            set 
            {
                _isEndOfQuiz = value;
                MakeWayForEndMessage = !IsEndOfQuiz;
                RaisePropertyChanged();
            }
        }


        private string _endMessage;

        public string EndMessage
        {
            get { return _endMessage; }
            set 
            {
                _endMessage = value;
                RaisePropertyChanged();
            }
        }


        private string myVar;

        public string Message
        {
            get { return myVar; }
            set { myVar = value; RaisePropertyChanged(); }
        }


        ObservableCollection<Question> RandomizedQuestions { get; set; }

        private ObservableCollection<string> _randomizedAnswers;

        public ObservableCollection<string> RandomizedAnswers
        {
            get { return _randomizedAnswers; }
            set 
            {
                _randomizedAnswers = value;
                RaisePropertyChanged();
            }
        }

        private int _timeLeft;

        public int TimeLeft
        {
            get { return _timeLeft; }
            set 
            {
                _timeLeft = value;
                RaisePropertyChanged();
            }
        }


        private DispatcherTimer _timer;

        public DispatcherTimer Timer
        {
            get { return _timer; }
            set 
            {
                _timer = value;
                RaisePropertyChanged();
            }
        }



        private string _answer;

        public string Answer
        {
            get { return _answer; }
            set { _answer = value; RaisePropertyChanged(); }
        }


        private Question _activeQuestion;

        public Question ActiveQuestion
        {
            get { return _activeQuestion; }
            set 
            { 
                _activeQuestion = value;
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

                if (value == true)
                {
                    StartPlayerView(null);
                }

            }
        }


        public DelegateCommand UpdateButtonCommand { get; }
        public DelegateCommand CheckAnswerCommand { get; }
        public DelegateCommand StartPlayerViewCommand { get; }



        private readonly MainWindowViewModel? mainWindowViewModel;
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            currentQuestionIndex = 0;


            IsPlayerVisible = false;
            IsEndOfQuiz = false;
            CheckAnswerCommand = new DelegateCommand(CheckAnswer);
            StartPlayerViewCommand = new DelegateCommand(StartPlayerView);

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;
            //timer.Start();

        }


        public void StartPlayerView(object? obj)
        {
            IsEndOfQuiz = false;
            correctAnswers = 0;
            currentQuestionIndex = 0;
            RandomizedQuestions = ActivePack.Questions;
            RandomizedQuestions.Shuffle();
            ActiveQuestion = RandomizedQuestions[currentQuestionIndex];
            RandomizeAnswers(ActiveQuestion);
            StartTimer();
        }
        public void RandomizeAnswers(Question currentQuestion)
        {
            RandomizedAnswers = new ObservableCollection<string>(currentQuestion.IncorrectAnswers);
            RandomizedAnswers.Add(currentQuestion.CorrectAnswer);
            RandomizedAnswers.Shuffle();
        }
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            TimeLeft--;

            if(TimeLeft < 1)
            {
                //set new question metod
                SetNewQuestion();
            }
        }

        public void StartTimer()
        {
            TimeLeft = ActivePack.TimeLimitInSeconds;
            Timer.Start();
            Message = $"Question {currentQuestionIndex + 1} of {RandomizedQuestions.Count}";
        }

        public void CheckAnswer(object? buttonAnswer)
        {
            if(buttonAnswer == ActiveQuestion.CorrectAnswer)
            {
                correctAnswers++;
            }
            else
            {

            }

            SetNewQuestion();
        }

        public void SetNewQuestion()
        {
            currentQuestionIndex++;

            if(currentQuestionIndex < RandomizedQuestions.Count)
            {
                StartTimer();
                ActiveQuestion = RandomizedQuestions[currentQuestionIndex];
                RandomizeAnswers(ActiveQuestion);
            }
            else
            {
                //TODO: Skriv antal frågor rätt fel
                EndMessage = $"You got {correctAnswers} right out of {RandomizedQuestions.Count}";
                IsEndOfQuiz = true;
            }
        }

    }
    public static class Extensions
    {
        public static ObservableCollection<T> Shuffle<T>(this ObservableCollection<T> questions)
        {
            Random rnd = new Random();
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = questions[k];
                questions[k] = questions[n];
                questions[n] = value;
            }

            return questions;
        }
    }
}
