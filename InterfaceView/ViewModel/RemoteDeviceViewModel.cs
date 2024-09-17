using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace InterfaceView.ViewModel
{
    public class RemoteDeviceViewModel : INotifyPropertyChanged
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
            set => SetOptions(nameof(IsActive), ref _isActive, value);
        }

        private IViewControl _parent;
        public IViewControl Parent
        {
            get => _parent;
            set => SetOptions(nameof(Parent), ref _parent, value);
        }

        public ObservableCollection<IViewControl> Elements { get; set; } = new ObservableCollection<IViewControl>();
        public ObservableCollection<NodeParam> NodeParams { get; set; }

        private double _temperature;
        public double Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature != value)
                {
                    _temperature = value;
                    OnPropertyChanged(nameof(Temperature));                    
                }
            }
        }

        private double _voltage;
        public double Voltage
        {
            get => _voltage;
            set
            {
                if (_voltage != value)
                {
                    _voltage = value;
                    OnPropertyChanged(nameof(Voltage));                    
                }
            }
        }

        private double _resistance;
        public double Resistance
        {
            get => _resistance;
            set
            {
                if (_resistance != value)
                {
                    _resistance = value;
                    OnPropertyChanged(nameof(Resistance));                    
                }
            }
        }

        public RemoteDeviceViewModel(string name, ObservableCollection<NodeParam> parameters)
        {
            ViewControlName = name;
            ViewControlType = "RemoteDevice";
            NodeParams = parameters;

            // Подписываемся на событие PropertyChanged для каждого параметра
            foreach (var param in NodeParams)
            {
                param.PropertyChanged += OnParamValueChanged;
            }

            // Инициализируем IsActive
            UpdateIsActive();
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetOptions<T>(string property, ref T variable, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(variable, value))
            {
                variable = value;
                OnPropertyChanged(property);
            }
        }
    }
}