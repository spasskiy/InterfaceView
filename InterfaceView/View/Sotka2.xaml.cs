using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InterfaceView.Model;
using LiveCharts;
using System.Timers;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для Sotka2.xaml
    /// </summary>
    public partial class Sotka2 : UserControl, IViewControl, INotifyPropertyChanged
    {
        // Коллекция для хранения истории значений ActiveNodes
        public ChartValues<int> ActiveNodesValues { get; set; }

        private string _viewControlType;
        public string ViewControlType
        {
            get => _viewControlType;
            set => SetOptions(nameof(ViewControlType), ref _viewControlType, value);
        }
        private string _viewControlName;
        public string ViewControlName
        {
            get => _viewControlName;
            set => SetOptions(nameof(ViewControlName), ref _viewControlName, value);
        }

        private IViewControl _parent;
        public IViewControl Parent
        {
            get => _parent;
            set => SetOptions(nameof(Parent), ref _parent, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetOptions(nameof(IsActive), ref _isActive, value);
        }

        private int _activeNodes;
        public int ActiveNodes
        {
            get => _activeNodes;
            set => SetOptions(nameof(ActiveNodes), ref _activeNodes, value);
        }

        private int _countOfNodes;
        public int CountOfNodes
        {
            get => _countOfNodes;
            set => SetOptions(nameof(CountOfNodes), ref _countOfNodes, value);
        }

        private IPAddress _ipAddress;
        public IPAddress IPAddress
        {
            get => _ipAddress;
            set => SetOptions(nameof(IPAddress), ref _ipAddress, value);
        }

        public ObservableCollection<IViewControl> Elements { get; set; }

        public Sotka2(string name, string ipAddress)
        {
            InitializeComponent();
            ViewControlName = name;
            ViewControlType = "Sotka2";
            DataContext = this;

            // Инициализация коллекции для графика
            ActiveNodesValues = new ChartValues<int>();

            // Инициализация таймера
            var updateTimer = new System.Timers.Timer(3000); // Обновление каждые 3 секунды
            updateTimer.Elapsed += OnUpdateTimerElapsed;
            updateTimer.AutoReset = true;
            updateTimer.Enabled = true;

            Elements = new ObservableCollection<IViewControl>();
            Elements.CollectionChanged += Elements_CollectionChanged;

            foreach (var element in Elements)
            {
                if (element is INotifyPropertyChanged notifyPropertyChangedElement)
                {
                    notifyPropertyChangedElement.PropertyChanged += Element_PropertyChanged;
                }
            }

            // Инициализация IP-адреса из строки
            IPAddress = new IPAddress(ipAddress);
        }

        private void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is INotifyPropertyChanged notifyPropertyChangedItem)
                    {
                        notifyPropertyChangedItem.PropertyChanged += Element_PropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    if (oldItem is INotifyPropertyChanged notifyPropertyChangedItem)
                    {
                        notifyPropertyChangedItem.PropertyChanged -= Element_PropertyChanged;
                    }
                }
            }

            CountOfNodes = Elements.Count;
            UpdateActiveNodes();
        }

        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsActive))
            {
                UpdateActiveNodes();
            }
        }

        private void UpdateActiveNodes()
        {
            ActiveNodes = Elements.Count(element => (element as dynamic).IsActive);
        }

        public void AddChildren(IViewControl element)
        {
            Elements.Add(element);
        }

        public Sotka2 Clone()
        {
            // Создаем новый объект Sotka2 с теми же параметрами
            var clone = new Sotka2(this.ViewControlName, this.IPAddress.ToString())
            {
                ViewControlType = this.ViewControlType,
                Parent = this.Parent, // Если Parent должен быть скопирован, иначе оставьте null
                IsActive = this.IsActive,
                ActiveNodes = this.ActiveNodes,
                CountOfNodes = this.CountOfNodes
            };

            // Копируем элементы коллекции Elements
            foreach (var element in this.Elements)
            {
                var cloneMethod = element.GetType().GetMethod("Clone");
                if (cloneMethod != null && cloneMethod.ReturnType != typeof(void))
                {
                    // Если метод Clone существует и возвращает значение, вызываем его
                    var clonedElement = cloneMethod.Invoke(element, null);
                    if (clonedElement is IViewControl clonedViewControl)
                    {
                        clone.AddChildren(clonedViewControl);
                    }
                }
                else
                {
                    // Если метод Clone отсутствует, добавляем элемент как есть
                    clone.AddChildren(element);
                }
            }

            return clone;
        }

        //Заморочки для отображения графика
        private void OnUpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Обновление данных на UI-потоке
            Dispatcher.Invoke(() =>
            {
                // Добавляем текущее количество активных узлов
                ActiveNodesValues.Add(ActiveNodes);

                // Ограничиваем количество точек на графике (например, последние 10 значений)
                if (ActiveNodesValues.Count > 10)
                {
                    ActiveNodesValues.RemoveAt(0);
                }
            });
        }
        // Форматирование оси X (время)
        public Func<double, string> XAxisFormatter => value => $"{value * 3} сек";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void SetOptions<T>(string Property, ref T variable, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(variable, value))
            {
                variable = value;
                OnPropertyChanged(new PropertyChangedEventArgs(Property));
            }
        }
        #endregion
    }
}