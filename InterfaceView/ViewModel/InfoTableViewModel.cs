using InterfaceView.View.Interfaces;
using InterfaceView.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace InterfaceView.ViewModel
{
    public class InfoTableViewModel : INotifyPropertyChanged
    {
        private List<IViewControl> _viewControls;
        private DispatcherTimer _updateTimer;

        private int _totalNodes;
        public int TotalNodes
        {
            get => _totalNodes;
            set
            {
                _totalNodes = value;
                OnPropertyChanged(nameof(TotalNodes));
            }
        }

        private int _sotka1Count;
        public int Sotka1Count
        {
            get => _sotka1Count;
            set
            {
                _sotka1Count = value;
                OnPropertyChanged(nameof(Sotka1Count));
            }
        }

        private int _remoteDeviceCount;
        public int RemoteDeviceCount
        {
            get => _remoteDeviceCount;
            set
            {
                _remoteDeviceCount = value;
                OnPropertyChanged(nameof(RemoteDeviceCount));
            }
        }

        private double _averageTemperature;
        public double AverageTemperature
        {
            get => _averageTemperature;
            set
            {
                _averageTemperature = value;
                OnPropertyChanged(nameof(AverageTemperature));
            }
        }

        private double _averageVoltage;
        public double AverageVoltage
        {
            get => _averageVoltage;
            set
            {
                _averageVoltage = value;
                OnPropertyChanged(nameof(AverageVoltage));
            }
        }

        private double _averageResistance;
        public double AverageResistance
        {
            get => _averageResistance;
            set
            {
                _averageResistance = value;
                OnPropertyChanged(nameof(AverageResistance));
            }
        }

        public InfoTableViewModel(List<IViewControl> viewControls)
        {
            _viewControls = viewControls;
            InitializeTimer();
            UpdateStatistics();
        }

        private void InitializeTimer()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(1); // Обновление каждую секунду
            _updateTimer.Tick += OnTimerTick;
            _updateTimer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdateStatistics();
        }

        public void UpdateStatistics()
        {
            TotalNodes = _viewControls.Count;
            Sotka1Count = _viewControls.Count(vc => vc is Sotka1);
            RemoteDeviceCount = _viewControls.Count(vc => vc is RemoteDevice);

            var remoteDeviceView = _viewControls.Where(vc => vc is RemoteDevice).Select(x => x as RemoteDevice).ToList();
            var remoteDevices = remoteDeviceView.Select(x => x.DataContext as RemoteDeviceViewModel);
            if (remoteDevices.Any())
            {
                AverageTemperature = remoteDevices.Average(rd => rd.NodeParams[0].ParamValue);
                AverageVoltage = remoteDevices.Average(rd => rd.NodeParams[1].ParamValue);
                AverageResistance = remoteDevices.Average(rd => rd.NodeParams[2].ParamValue);
            }
            else
            {
                AverageTemperature = 0;
                AverageVoltage = 0;
                AverageResistance = 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}