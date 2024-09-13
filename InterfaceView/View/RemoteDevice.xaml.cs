using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для RemoteDevice.xaml
    /// </summary>
    public partial class RemoteDevice : UserControl, IViewControl, INotifyPropertyChanged
    {
        private string _viewControlName;
        public string ViewControlName
        {
            get => _viewControlName;
            set => SetOptions(nameof(ViewControlName), ref _viewControlName, value);
        }
        private string _viewControlType;
        public string ViewControlType
        {
            get => _viewControlType;
            set => SetOptions(nameof(ViewControlType), ref _viewControlType, value);
        }


        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set 
            {
                SetOptions(nameof(IsActive), ref _isActive, value);
                if(Parent != null)
                {
                    Parent.IsActive = value;
                }
            }

        }

        private IViewControl _parent;
        public IViewControl Parent
        {
            get => _parent;
            set => SetOptions(nameof(Parent), ref _parent, value);
        }

        public ObservableCollection<IViewControl> Elements { get; set; } = new ObservableCollection<IViewControl>();
        public ObservableCollection<NodeParam> NodeParams { get; set; }

        public RemoteDevice(string name, ObservableCollection<NodeParam> Params)
        {
            ViewControlName = name;
            ViewControlType = "RemoteDevice";
            Elements = new();
            NodeParams = Params;
            FillGrid(ParamsGrid);
            InitializeComponent();
            DataContext = this;

            // Подписываемся на событие PropertyChanged для каждого параметра
            foreach (var param in NodeParams)
            {
                param.PropertyChanged += OnParamValueChanged;
            }

            // Инициализируем IsActive
            UpdateIsActive();
        }

        public void FillGrid(Grid grid)
        {
            if (grid == null) return;

            // Очистка существующих строк
            grid.Children.Clear();
            grid.RowDefinitions.Clear();

            // Добавление строк для каждого параметра
            for (int i = 0; i < NodeParams.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var rowGrid = new Grid();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var paramNameTextBlock = new TextBlock { Text = NodeParams[i].ParamName };
                var paramValueTextBlock = new TextBlock { Text = NodeParams[i].ParamValue.ToString() };
                var measureUnitTextBlock = new TextBlock { Text = NodeParams[i].MeasureUnit };

                Grid.SetRow(rowGrid, i);
                Grid.SetColumn(paramNameTextBlock, 0);
                Grid.SetColumn(paramValueTextBlock, 1);
                Grid.SetColumn(measureUnitTextBlock, 2);

                rowGrid.Children.Add(paramNameTextBlock);
                rowGrid.Children.Add(paramValueTextBlock);
                rowGrid.Children.Add(measureUnitTextBlock);

                grid.Children.Add(rowGrid);
            }
        }

        public void AddChildren(IViewControl element)
        {
            Elements.Add(element);
        }

        private void OnParamValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NodeParam.ParamValue))
            {
                UpdateIsActive();
            }
        }

        private void UpdateIsActive()
        {
            IsActive = NodeParams.All(param => param.ParamValue >= 0);
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