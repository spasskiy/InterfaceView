using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для BottomStatusBar.xaml
    /// </summary>
    public partial class BottomStatusBar : UserControl, INotifyPropertyChanged
    {
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            private set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }

        private int _availableNodesCount = 10;
        public int AvailableNodesCount
        {
            get => _availableNodesCount;
            private set
            {
                if (_availableNodesCount != value)
                {
                    _availableNodesCount = value;
                    OnPropertyChanged(nameof(AvailableNodesCount));
                }
            }
        }

        private int _unavailableNodesCount = 0;
        public int UnavailableNodesCount
        {
            get => _unavailableNodesCount;
            private set
            {
                if (_unavailableNodesCount != value)
                {
                    _unavailableNodesCount = value;
                    OnPropertyChanged(nameof(UnavailableNodesCount));
                }
            }
        }

        private int _sentRequestsCount = 0;
        public int SentRequestsCount
        {
            get => _sentRequestsCount;
            private set
            {
                if (_sentRequestsCount != value)
                {
                    _sentRequestsCount = value;
                    OnPropertyChanged(nameof(SentRequestsCount));
                }
            }
        }

        private int _receivedRequestsCount = 0;
        public int ReceivedRequestsCount
        {
            get => _receivedRequestsCount;
            private set
            {
                if (_receivedRequestsCount != value)
                {
                    _receivedRequestsCount = value;
                    OnPropertyChanged(nameof(ReceivedRequestsCount));
                }
            }
        }

        private Random _random = new Random();

        public BottomStatusBar()
        {
            InitializeComponent();
            DataContext = this;

            // Инициализация таймера для обновления времени
            var timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                CurrentTime = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");
            }, Dispatcher.CurrentDispatcher);

            // Запуск асинхронных задач для увеличения количества запросов
            Task.Run(async () => await IncreaseReceivedRequestsAsync());
            Task.Run(async () => await IncreaseSentRequestsAsync());
        }

        private async Task IncreaseReceivedRequestsAsync()
        {
            while (true)
            {
                await Task.Delay(_random.Next(1000, 3000)); // Рандомный интервал от 1 до 3 секунд
                Dispatcher.Invoke(() => ReceivedRequestsCount++);
            }
        }

        private async Task IncreaseSentRequestsAsync()
        {
            while (true)
            {
                await Task.Delay(_random.Next(1000, 5000)); // Рандомный интервал от 1 до 5 секунд
                Dispatcher.Invoke(() => SentRequestsCount++);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}