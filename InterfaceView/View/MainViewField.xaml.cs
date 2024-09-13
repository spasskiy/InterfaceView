using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
            LoadFromXml("canvasData.xml");
        }

        private void LoadFromXml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // Создаем файл с базовыми данными, если он не существует
                var canvasData = new CanvasData
                {
                    Controls = new List<ViewControlData>
            {
                new ViewControlData
                {
                    Name = "Sotka2",
                    Left = 50,
                    Top = 50,
                    IsActive = true
                },
                new ViewControlData
                {
                    Name = "Sotka1",
                    Left = 150,
                    Top = 150,
                    IsActive = true,
                    ParentName = "Sotka2"
                },
                new ViewControlData
                {
                    Name = "RemoteDevice",
                    Left = 250,
                    Top = 250,
                    IsActive = true,
                    ParentName = "Sotka1"
                }
            }
                };

                SerializationHelper.SerializeToFile(canvasData, filePath);
            }

            var loadedCanvasData = SerializationHelper.DeserializeFromFile<CanvasData>(filePath);
            if (loadedCanvasData != null)
            {
                foreach (var controlData in loadedCanvasData.Controls)
                {
                    CreateControl(controlData, canvas);
                }
                UpdateLines();
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLines();
        }

        private IViewControl CreateControl(ViewControlData controlData, Canvas canvas, IViewControl parent = null)
        {
            IViewControl control = null;
            switch (controlData.Name)
            {
                case "Sotka2":
                    control = new Sotka2(controlData.Name, controlData.IPAddress); // Используем новый конструктор
                    break;
                case "Sotka1":
                    control = new Sotka1(controlData.Name, controlData.IPAddress);
                    break;
                case "RemoteDevice":
                    control = new RemoteDevice(controlData.Name, new ObservableCollection<NodeParam>());
                    break;
                    // Добавьте другие типы контролов, если необходимо
            }

            if (control != null)
            {
                Canvas.SetLeft(control as FrameworkElement, controlData.Left);
                Canvas.SetTop(control as FrameworkElement, controlData.Top);
                control.IsActive = controlData.IsActive;

                // Устанавливаем родительский элемент
                control.Parent = parent;

                foreach (var childData in controlData.Children)
                {
                    var child = CreateControl(childData, canvas, control);
                    if (child != null)
                    {
                        control.AddChildren(child);
                    }
                }

                // Подключаем события для перетаскивания
                var frameworkElement = control as FrameworkElement;
                if (frameworkElement != null)
                {
                    frameworkElement.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
                    frameworkElement.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
                    frameworkElement.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
                }

                // Добавляем элемент на Canvas
                canvas.Children.Add(control as UIElement);
                _viewControls.Add(control);
            }

            return control;
        }


        public void SaveToXml(string filePath)
        {
            var canvasData = new CanvasData();
            foreach (var control in _viewControls)
            {
                var controlData = new ViewControlData
                {
                    Name = control.ViewControlName,
                    Left = Canvas.GetLeft(control as FrameworkElement),
                    Top = Canvas.GetTop(control as FrameworkElement),
                    IsActive = control.IsActive,
                    ParentName = control.Parent?.ViewControlName
                };

                // Проверка на тип контрола перед сохранением IP-адреса
                if (control is Sotka2 || control is Sotka1)
                {
                    controlData.IPAddress = (control as dynamic).IPAddress.ToString();
                }

                foreach (var child in control.Elements)
                {
                    var childData = new ViewControlData
                    {
                        Name = child.ViewControlName,
                        Left = Canvas.GetLeft(child as FrameworkElement),
                        Top = Canvas.GetTop(child as FrameworkElement),
                        IsActive = child.IsActive,
                        ParentName = child.Parent?.ViewControlName
                    };

                    // Проверка на тип контрола перед сохранением IP-адреса
                    if (child is Sotka2 || child is Sotka1)
                    {
                        childData.IPAddress = (child as dynamic).IPAddress.ToString();
                    }

                    controlData.Children.Add(childData);
                }

                canvasData.Controls.Add(controlData);
            }

            SerializationHelper.SerializeToFile(canvasData, filePath);
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