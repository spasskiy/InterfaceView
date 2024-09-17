using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace InterfaceView.ViewModel
{
    public class ConsoleViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public ConsoleViewModel()
        {
            _messages = new ObservableCollection<string>();
            InitializeConsole();
        }

        private async void InitializeConsole()
        {
            await Task.Delay(2000); // Задержка перед стартовыми сообщениями
            AddMessage("Консоль инициализирована.");
            await Task.Delay(3000);
            AddMessage("Запрос отправлен.");
            await Task.Delay(3000);
            AddMessage("Ожидание ответа...");
            await Task.Delay(3000);
            AddMessage("Запрос принят.");
            await Task.Delay(3000);
            AddMessage("Узлы инициализированы.");
        }

        private void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}