using InterfaceView.View.Interfaces;
using InterfaceView.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace InterfaceView.ViewModel
{
    public class InfoTableViewModel : INotifyPropertyChanged
    {
        private List<IViewControl> _viewControls;

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
            UpdateStatistics();
        }

        public void UpdateStatistics()
        {
            TotalNodes = _viewControls.Count;
            Sotka1Count = _viewControls.Count(vc => vc is Sotka1);
            RemoteDeviceCount = _viewControls.Count(vc => vc is RemoteDevice);

            var remoteDevices = _viewControls.OfType<RemoteDevice>();
            if (remoteDevices.Any())
            {
                //AverageTemperature = remoteDevices.Average(rd => rd.Temperature);
                //AverageVoltage = remoteDevices.Average(rd => rd.Voltage);
                //AverageResistance = remoteDevices.Average(rd => rd.Resistance);
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