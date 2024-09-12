using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для MainViewField.xaml
    /// Основной контрол отображения. Пока вся логика работы в нём.
    /// </summary>
    public partial class MainViewField : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;
        private List<Line> _lines = new List<Line>();
        private List<IViewControl> _viewControls = new List<IViewControl>();

        public MainViewField()
        {
            InitializeComponent();

            var sotka = new Sotka2("ЛПУ");
            Canvas.SetLeft(sotka, 50);
            Canvas.SetTop(sotka, 50);
            sotka.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            sotka.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            sotka.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
            canvas.Children.Add(sotka);

            var sotka1 = new Sotka1("ЛЭС1");
            sotka1.Parent = sotka;
            Canvas.SetLeft(sotka1, 150);
            Canvas.SetTop(sotka1, 150);
            sotka1.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            sotka1.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            sotka1.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
            canvas.Children.Add(sotka1);

            var device1 = new RemoteDevice("BKM1", new ObservableCollection<NodeParam>
            {
                new NodeParam("Напряжение", 10, "V"),
                new NodeParam("Сопротивление", 20, "Ω"),
                new NodeParam("Температура", 30, "℃")
            });
            device1.Parent = sotka1;
            Canvas.SetLeft(device1, 250);
            Canvas.SetTop(device1, 250);
            device1.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            device1.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            device1.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
            canvas.Children.Add(device1);

            // Собираем детей с IViewControl
            foreach (var view in canvas.Children)
            {
                if (view is IViewControl viewControl)
                {
                    _viewControls.Add(viewControl);
                    viewControl.PropertyChanged += OnViewControlPropertyChanged;
                }
            }

            UpdateLines();
        }

        private void FrameworkElement_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            var draggableControl = sender as FrameworkElement;
            _startPoint = e.GetPosition(draggableControl);
            draggableControl.CaptureMouse();
        }

        private void FrameworkElement_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var draggableControl = sender as FrameworkElement;
                Point currentPoint = e.GetPosition(this.canvas);
                double newLeft = currentPoint.X - _startPoint.X;
                double newTop = currentPoint.Y - _startPoint.Y;

                // Ограничение перемещения внутри Canvas с отступом
                if (newLeft < 0) newLeft = 0;
                if (newTop < 0) newTop = 0;
                if (newLeft + draggableControl.ActualWidth > canvas.ActualWidth)
                    newLeft = canvas.ActualWidth - draggableControl.ActualWidth;
                if (newTop + draggableControl.ActualHeight > canvas.ActualHeight - 1) // Отступ 1 пиксель
                    newTop = canvas.ActualHeight - draggableControl.ActualHeight - 3;

                Canvas.SetLeft(draggableControl, newLeft);
                Canvas.SetTop(draggableControl, newTop);

                UpdateLines();
            }
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var draggableControl = sender as FrameworkElement;
            draggableControl.ReleaseMouseCapture();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLines();
        }

        private void UpdateLines()
        {
            // Очищаем предыдущие линии
            foreach (var line in _lines)
            {
                canvas.Children.Remove(line);
            }
            _lines.Clear();

            DrawLines(canvas, _viewControls);
        }

        private void DrawLines(Canvas canvas, IEnumerable<IViewControl> controls)
        {
            if (controls == null || controls.Count() == 0) return;
            foreach (var control in controls)
            {
                if (control.Parent != null)
                {
                    var parentControl = control.Parent as FrameworkElement;
                    var childControl = control as FrameworkElement;

                    if (parentControl != null && childControl != null)
                    {
                        double parentLeft = Canvas.GetLeft(parentControl);
                        double parentTop = Canvas.GetTop(parentControl);
                        double childLeft = Canvas.GetLeft(childControl);
                        double childTop = Canvas.GetTop(childControl);

                        // Проверка на корректность значений
                        if (double.IsNaN(parentLeft) || double.IsNaN(parentTop) || double.IsNaN(childLeft) || double.IsNaN(childTop))
                        {
                            continue; // Пропускаем этот элемент, если значения некорректны
                        }

                        var line = new Line
                        {
                            X1 = parentLeft + parentControl.ActualWidth / 2,
                            Y1 = parentTop + parentControl.ActualHeight / 2,
                            X2 = childLeft + childControl.ActualWidth / 2,
                            Y2 = childTop + childControl.ActualHeight / 2,
                            StrokeThickness = 2
                        };

                        // Устанавливаем цвет линии в зависимости от IsActive
                        line.Stroke = control.IsActive ? Brushes.Blue : Brushes.Red;

                        canvas.Children.Insert(0, line); // Добавляем линию в начало коллекции
                        _lines.Add(line);
                    }
                }

                // Рекурсивно обрабатываем дочерние элементы
                DrawLines(canvas, control.Elements);
            }
        }

        private void OnViewControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IViewControl.IsActive))
            {
                UpdateLines();
            }
        }
    }
}