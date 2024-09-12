using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

namespace InterfaceView.Model
{
    public class NodeParam : INotifyPropertyChanged
    {
        private string _paramName;
        public string ParamName
        {
            get { return _paramName; }
            set { SetOptions(nameof(ParamName), ref _paramName, value); }
        }

        private double _paramValue;
        public double ParamValue
        {
            get { return _paramValue; }
            set
            {
                SetOptions(nameof(ParamValue), ref _paramValue, value);
                if (value < 0) IsFine = false;
                else IsFine = true;
            }
        }

        private bool _isFine;
        public bool IsFine
        {
            get { return _isFine; }
            set { SetOptions(nameof(IsFine), ref _isFine, value); }
        }

        private string _measureUnit;
        public string MeasureUnit
        {
            get { return _measureUnit; }
            set { SetOptions(nameof(MeasureUnit), ref _measureUnit, value); }
        }

        private DispatcherTimer _timer;
        private double _targetValue;
        private double _step;

        public NodeParam(string paramName, double paramValue, string measureUnit)
        {
            ParamName = paramName;
            ParamValue = paramValue;
            MeasureUnit = measureUnit;

            // Инициализация таймера
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // 1 секунда
            _timer.Tick += OnTimerTick;
            _timer.Start();

            // Инициализация начальных значений
            RandomizeTargetValue();
            RandomizeStep();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // Изменение значения ParamValue
            if (Math.Abs(Math.Abs(ParamValue) - Math.Abs(_targetValue)) > Math.Abs(_step))
            {
                ParamValue += _step;
            }
            else
            {
                ParamValue = _targetValue;
                RandomizeTargetValue();
                RandomizeStep();
            }
        }

        private void RandomizeTargetValue()
        {
            // Генерация нового целевого значения
            Random random = new Random();
            double randomValue = random.NextDouble() * 150 - 50; // Диапазон от -50 до 100

            // Увеличение вероятности положительных значений
            if (random.NextDouble() < 0.75)
            {
                randomValue = Math.Abs(randomValue); // Убедимся, что значение положительное
            }

            // Проверка и корректировка значения, если оно выходит за пределы допустимого диапазона
            if (randomValue < -50)
            {
                randomValue = random.Next(10, 101); // Диапазон от 10 до 100
            }
            else if (randomValue > 100)
            {
                randomValue = random.Next(-50, 51); // Диапазон от -50 до 50
            }

            _targetValue = randomValue;
        }


        private void RandomizeStep()
        {
            // Генерация нового шага
            Random random = new Random();
            _step = (random.NextDouble() * 10) * (ParamValue < _targetValue ? 1 : -1);
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
