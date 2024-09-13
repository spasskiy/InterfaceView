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

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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

        public void SaveToXml(string filePath)
        {
            var canvasData = new CanvasData();
            var rootControls = new List<IViewControl>();

            // Находим все корневые элементы (те, у которых нет родителя)
            foreach (var control in _viewControls)
            {
                if (control.Parent == null)
                {
                    rootControls.Add(control);
                }
            }

            // Рекурсивно сохраняем иерархию
            foreach (var rootControl in rootControls)
            {
                var controlData = SaveControlHierarchy(rootControl);
                canvasData.Controls.Add(controlData);
            }

            SerializationHelper.SerializeToFile(canvasData, filePath);
        }

        private ViewControlData SaveControlHierarchy(IViewControl control)
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

            // Сохраняем NodeParams, если это RemoteDevice
            if (control is RemoteDevice remoteDevice)
            {
                foreach (var nodeParam in remoteDevice.NodeParams)
                {
                    var nodeParamData = new NodeParamData
                    {
                        ParamName = nodeParam.ParamName,
                        ParamValue = nodeParam.ParamValue,
                        MeasureUnit = nodeParam.MeasureUnit
                    };
                    controlData.NodeParams.Add(nodeParamData);
                }
            }

            // Рекурсивно сохраняем детей
            foreach (var child in control.Elements)
            {
                var childData = SaveControlHierarchy(child);
                controlData.Children.Add(childData);
            }

            return controlData;
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
                            IsActive = true,
                            IPAddress = "10.10.11.21" // Добавлен IP-адрес
                        },
                        new ViewControlData
                        {
                            Name = "Sotka1",
                            Left = 150,
                            Top = 150,
                            IsActive = true,
                            ParentName = "Sotka2",
                            IPAddress = "10.10.12.11" // Добавлен IP-адрес
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
                // Создаем словарь для хранения созданных контролов
                var controlDictionary = new Dictionary<string, IViewControl>();

                // Сначала создаем все контролы без установки родителей
                foreach (var controlData in loadedCanvasData.Controls)
                {
                    var control = CreateControl(controlData, canvas);
                    if (control != null)
                    {
                        controlDictionary[controlData.Name] = control;
                    }
                }

                // Затем устанавливаем родителей для всех контролов
                foreach (var controlData in loadedCanvasData.Controls)
                {
                    if (controlData.ParentName != null)
                    {
                        var control = controlDictionary[controlData.Name];
                        var parent = controlDictionary[controlData.ParentName];
                        control.Parent = parent;
                        parent.AddChildren(control);
                    }
                }

                UpdateLines();
            }
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
                    control = new Sotka1(controlData.Name, controlData.IPAddress); // Используем новый конструктор
                    break;
                case "RemoteDevice":
                    // Создаем NodeParams для каждого RemoteDevice
                    var nodeParams = new ObservableCollection<NodeParam>();
                    foreach (var nodeParamData in controlData.NodeParams)
                    {
                        var nodeParam = new NodeParam(nodeParamData.ParamName, nodeParamData.ParamValue, nodeParamData.MeasureUnit);
                        nodeParams.Add(nodeParam);
                    }
                    control = new RemoteDevice(controlData.Name, nodeParams);
                    break;
                    // Добавьте другие типы контролов, если необходимо
            }

            if (control != null)
            {
                Canvas.SetLeft(control as FrameworkElement, controlData.Left);
                Canvas.SetTop(control as FrameworkElement, controlData.Top);
                control.IsActive = controlData.IsActive;
                control.Parent = parent;

                var frameworkElement = control as FrameworkElement;
                if (frameworkElement != null)
                {
                    frameworkElement.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
                    frameworkElement.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
                    frameworkElement.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
                }

                canvas.Children.Add(control as UIElement);
                _viewControls.Add(control);

                // Подписываемся на событие PropertyChanged
                control.PropertyChanged += OnViewControlPropertyChanged;

                // Добавляем детей
                if (controlData.Children != null)
                {
                    foreach (var childData in controlData.Children)
                    {
                        var child = CreateControl(childData, canvas, control);
                        control.AddChildren(child);
                    }
                }
            }

            return control;
        }
    }
}