using Labb03_QuizApplication.Command;
using Labb03_QuizApplication.JsonHandler;
using Labb03_QuizApplication.Model;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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


        private bool _isBeginningOfQuiz;

        public bool IsBeginningOfQuiz
        {
            get { return _isBeginningOfQuiz; }
            set { _isBeginningOfQuiz = value; RaisePropertyChanged(); }
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

        private bool _hasAnswered;

        public bool HasAnswered
        {
            get { return _hasAnswered; }
            set 
            {
                _hasAnswered = value;
                RaisePropertyChanged();
            }
        }



        private string _message;

        public string QuestionMessage
        {
            get { return _message; }
            set { _message = value; RaisePropertyChanged(); }
        }


        List<UserViewModel> Users { get; set; }
        List<AnswerViewModel> Answers { get; set; }
        public ObservableCollection<int> AmountAnswered { get; set; }
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

        private ObservableCollection<string> _answerColors;

        public ObservableCollection<string> AnswerColors
        {
            get { return _answerColors; }
            set { _answerColors = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<UserViewModel> _filteredUsers;

        public ObservableCollection<UserViewModel> FilteredUsers
        {
            get { return _filteredUsers; }
            set { _filteredUsers = value; RaisePropertyChanged(); }
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

        private UserViewModel _activeUser;

        public UserViewModel ActiveUser
        {
            get { return _activeUser; }
            set { _activeUser = value; RaisePropertyChanged(); }
        }

        private string _newUserName;

        public string NewUserName
        {
            get { return _newUserName; }
            set { _newUserName = value; RaisePropertyChanged(); }
        }




        public DelegateCommand UpdateButtonCommand { get; }
        public DelegateCommand CheckAnswerCommand { get; }
        public DelegateCommand StartPlayerViewCommand { get; }
        public DelegateCommand AddUserCommand { get; }



        private readonly MainWindowViewModel? mainWindowViewModel;
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            currentQuestionIndex = 0;


            IsPlayerVisible = false;
            IsEndOfQuiz = false;
            HasAnswered = false;
            AnswerColors = new ObservableCollection<string> { "White", "White", "White", "White", };

            GetUsersAndAnswers();

            CheckAnswerCommand = new DelegateCommand(CheckAnswer);
            StartPlayerViewCommand = new DelegateCommand(StartPlayerView);
            AddUserCommand = new DelegateCommand(AddUser);

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += Timer_Tick;

        }

        public void EndQuiz()
        {
            EndMessage = $"You got {correctAnswers} right out of {RandomizedQuestions.Count}";

            if(Users.Any(u => u != ActiveUser))
            {
                Users.Add(ActiveUser);
                var filter = Users
                    .Where(u => u.UserPack.Id == ActiveUser.UserPack.Id)
                    .OrderByDescending(u => u.Score)
                    .ThenBy(u => u.Time)
                    .Take(5);

                FilteredUsers = new ObservableCollection<UserViewModel>(filter);
                RaisePropertyChanged("FilteredUsers");

                FileReader.SaveUser(ActiveUser);
            }

            IsEndOfQuiz = true;
        }
        public async Task GetUsersAndAnswers()
        {
            Users = await FileReader.LoadUsersAsync();
            RaisePropertyChanged("Users");
            Answers = await FileReader.GetAnswersAsync();
        }
        //TODO: gör så att när man trycker på RETRY knapp så får man skriva in nytt användarnamn och så skapas nytt namn
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

            AmountAnswered = new ObservableCollection<int>();
            RaisePropertyChanged("AmountAnswered");

            foreach (string s in RandomizedAnswers)
            {
                try
                {
                    // TODO: Om man har två svar som är likadana på två olika frågor så kommer den att samla svaren på båda frågorna som ett, fixa detta.
                    AmountAnswered.Add(Answers.Where(a => a.Answer == s && a.BelongingQuestionPack.Name == ActivePack.Name ).ToList().Count);
                }
                catch
                {
                    AmountAnswered.Add(0);
                }
            }
        }
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            TimeLeft--;
            ActiveUser.Time++;

            if(TimeLeft < 1)
            {
                Timer.Stop();

                int indexOfCorrectAnswer = RandomizedAnswers.IndexOf(ActiveQuestion.CorrectAnswer);
                await SetAnswersColor(indexOfCorrectAnswer, indexOfCorrectAnswer, true);
                SetNewQuestion();


            }
        }

        public void StartTimer()
        {
            TimeLeft = ActivePack.TimeLimitInSeconds;
            Timer.Start();
            QuestionMessage = $"Question {currentQuestionIndex + 1} of {RandomizedQuestions.Count}";
        }

        public async void CheckAnswer(object? buttonAnswer)
        {

            int indexOfAnswer = Int32.Parse((string)buttonAnswer);
            int indexOfCorrectAnswer = RandomizedAnswers.IndexOf(ActiveQuestion.CorrectAnswer);
            Timer.Stop();

            //AmountAnswered = new ObservableCollection<int>();
            //RaisePropertyChanged("AmountAnswered");

            //foreach (string s in RandomizedAnswers)
            //{
            //    try
            //    {
            //        AmountAnswered.Add(Answers.Where(a => a.Answer == s && a.BelongingQuestionPack.Name == ActivePack.Name).ToList().Count);
            //    }
            //    catch
            //    {
            //        AmountAnswered.Add(0);
            //    }
            //}

            if (HasAnswered == false)
            {
                HasAnswered = true;

                if (RandomizedAnswers[indexOfAnswer] == RandomizedAnswers[indexOfCorrectAnswer])
                {
                    correctAnswers++;
                    ActiveUser.Score++;
                    await SetAnswersColor(indexOfCorrectAnswer, indexOfAnswer, true);
                }
                else
                {
                    await SetAnswersColor(indexOfCorrectAnswer, indexOfAnswer, false);
                }

                var newAnswer = new AnswerViewModel()
                {
                    Answer = RandomizedAnswers.ElementAt(indexOfAnswer),
                    Id = ObjectId.GenerateNewId(),
                    BelongingQuestionPack = ActivePack
                };
                FileReader.AddAnswerToDb(newAnswer);

                SetNewQuestion();
                HasAnswered = false;

            }
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
                EndQuiz();
            }
        }

        public async Task SetAnswersColor(int correctAnswerIndex, int answerIndex, bool isEquals)
        {
            if(isEquals == true)
            {
                AnswerColors[correctAnswerIndex] = "Green";
                await Task.Delay(2000);
                AnswerColors[correctAnswerIndex] = "White";
            }
            else 
            {
                AnswerColors[correctAnswerIndex] = "Green";
                AnswerColors[answerIndex] = "Red";
                await Task.Delay(2000);
                AnswerColors[correctAnswerIndex] = "White";
                AnswerColors[answerIndex] = "White";

            }

        }

        public void AddUser(object parameter)
        {
            ActiveUser = new UserViewModel() 
            { 
                Name = NewUserName, 
                UserPack = ActivePack, 
                Score = 0, 
                Time = 0, 
                Id = ObjectId.GenerateNewId() 
            };

            IsBeginningOfQuiz = false;
            IsPlayerVisible = true;
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
