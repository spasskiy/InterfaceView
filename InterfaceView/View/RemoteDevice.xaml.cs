using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using InterfaceView.ViewModel;

namespace InterfaceView.View
{
    public partial class RemoteDevice : UserControl, IViewControl, INotifyPropertyChanged
    {
        private RemoteDeviceViewModel _viewModel;

        public string ViewControlName
        {
            get => _viewModel.ViewControlName;
            set => _viewModel.ViewControlName = value;
        }

        public string ViewControlType
        {
            get => _viewModel.ViewControlType;
            set => _viewModel.ViewControlType = value;
        }

        public bool IsActive
        {
            get => _viewModel.IsActive;
            set => _viewModel.IsActive = value;
        }

        public IViewControl Parent
        {
            get => _viewModel.Parent;
            set => _viewModel.Parent = value;
        }

        public ObservableCollection<IViewControl> Elements
        {
            get => _viewModel.Elements;
            set => _viewModel.Elements = value;
        }

        public ObservableCollection<NodeParam> NodeParams
        {
            get => _viewModel.NodeParams;
            set => _viewModel.NodeParams = value;
        }

        public RemoteDevice(string name, ObservableCollection<NodeParam> parameters)
        {
            _viewModel = new RemoteDeviceViewModel(name, parameters);
            DataContext = _viewModel;
            InitializeComponent();

            // Подписываемся на событие PropertyChanged для ViewModel
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public void AddChildren(IViewControl element)
        {
            Elements.Add(element);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        #endregion
    }
}