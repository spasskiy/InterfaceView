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

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для Sotka2.xaml
    /// </summary>
    public partial class Sotka2 : UserControl, IViewControl, INotifyPropertyChanged
    {
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