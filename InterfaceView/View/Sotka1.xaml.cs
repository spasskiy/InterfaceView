using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using InterfaceView.Model;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для Sotka1.xaml
    /// </summary>
    public partial class Sotka1 : UserControl, IViewControl, INotifyPropertyChanged
    {
        private string _viewControlName;
        public string ViewControlName
        {
            get => _viewControlName;
            set => SetOptions(nameof(ViewControlName), ref _viewControlName, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetOptions(nameof(IsActive), ref _isActive, value);
        }

        private IViewControl _parent;
        public IViewControl Parent
        {
            get => _parent;
            set => SetOptions(nameof(Parent), ref _parent, value);
        }

        private IPAddress _ipAddress;
        public IPAddress IPAddress
        {
            get => _ipAddress;
            set => SetOptions(nameof(IPAddress), ref _ipAddress, value);
        }

        public ObservableCollection<IViewControl> Elements { get; set; }

        private int _countOfNodes;
        public int CountOfNodes
        {
            get => _countOfNodes;
            private set => SetOptions(nameof(CountOfNodes), ref _countOfNodes, value);
        }

        private int _activeNodes;
        public int ActiveNodes
        {
            get => _activeNodes;
            set => SetOptions(nameof(ActiveNodes), ref _activeNodes, value);
        }

        public Sotka1(string name, string ipAddress)
        {
            ViewControlName = name;
            IPAddress = new IPAddress(ipAddress);
            Elements = new ObservableCollection<IViewControl>();
            IsActive = false;
            InitializeComponent();
            DataContext = this;

            // Подписываемся на событие CollectionChanged коллекции Elements
            Elements.CollectionChanged += Elements_CollectionChanged;
        }

        private void Elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateCounts();
        }

        public void AddChildren(IViewControl element)
        {
            Elements.Add(element);
            element.PropertyChanged += Child_PropertyChanged;
            UpdateCounts();
        }

        private void Child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IViewControl.IsActive))
            {
                UpdateCounts();
            }
        }

        private void UpdateCounts()
        {
            CountOfNodes = Elements.Count;
            ActiveNodes = Elements.Count(e => e.IsActive);
            IsActive = CountOfNodes == ActiveNodes;
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