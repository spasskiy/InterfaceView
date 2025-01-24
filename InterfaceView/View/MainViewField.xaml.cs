using InterfaceView.Model;
using InterfaceView.View.Interfaces;
using InterfaceView.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace InterfaceView.View
{
    public partial class MainViewField : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;
        private bool _isLinesDirty = false; // Флаг, указывающий, что линии нужно обновить
        private DateTime _lastClickTime; // Время последнего клика для отслеживания даблклика

        private List<Line> _lines = new List<Line>();
        private List<IViewControl> _viewControls = new List<IViewControl>();
        private InfoTableViewModel _infoTableViewModel;

        public MainViewField()
        {
            InitializeComponent();
            LoadFromXml("canvasData.xml");
            AddInfoTable();
        }

        private void AddInfoTable()
        {
            _infoTableViewModel = new InfoTableViewModel(_viewControls);
            var infoTable = new InfoTableControl();
            infoTable.DataContext = _infoTableViewModel;
            double rightMargin = 10;
            double topMargin = 10;

            // Рассчитываем позицию для прижатия к правому верхнему углу
            double leftPosition = canvas.ActualWidth - infoTable.ActualWidth + rightMargin;
            double topPosition = topMargin;

            Canvas.SetRight(infoTable, leftPosition);
            Canvas.SetTop(infoTable, topPosition);
            canvas.Children.Add(infoTable);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _isLinesDirty = true;
            UpdateLinesIfNeeded();
            UpdateCanvasClip();
        }

        private void UpdateCanvasClip()
        {
            if (canvas != null)
            {
                var clipRect = new Rect(0, 0, canvas.ActualWidth, canvas.ActualHeight);
                var rectangleGeometry = new RectangleGeometry(clipRect);
                canvas.Clip = rectangleGeometry;
            }
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

                _isLinesDirty = true;
                UpdateLinesIfNeeded();
            }
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var draggableControl = sender as FrameworkElement;
            draggableControl.ReleaseMouseCapture();
        }

        private void UpdateLinesIfNeeded()
        {
            if (_isLinesDirty)
            {
                UpdateLines();
                _isLinesDirty = false;
            }
        }

        private void UpdateLines()
        {
            //MethodCallLogger.LogMethodCall(nameof(UpdateLines));
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
            //MethodCallLogger.LogMethodCall(nameof(DrawLines));
            if (controls == null || controls.Count() == 0) return;
            foreach (var control in controls)
            {
                if (control.Parent != null)
                {
                    var parentControl = control.Parent as FrameworkElement;
                    var childControl = control as FrameworkElement;

                    if (parentControl != null && childControl != null)
                    {
                        // Проверка на видимость контролов
                        if (parentControl.Visibility != Visibility.Visible || childControl.Visibility != Visibility.Visible)
                        {
                            continue; // Пропускаем этот элемент, если хотя бы один из контролов скрыт
                        }

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
            }
        }

        private void OnViewControlPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //MethodCallLogger.LogMethodCall(nameof(OnViewControlPropertyChanged));
            if (e.PropertyName == nameof(IViewControl.IsActive))
            {
                _isLinesDirty = true;
                UpdateLinesIfNeeded();
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
                ViewControlType = control.ViewControlType,
                ViewControlName = control.ViewControlName,
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
                    ViewControlType = "Sotka2",
                    ViewControlName = "Sotka2",
                    Left = 50,
                    Top = 50,
                    IsActive = true,
                    IPAddress = "10.10.11.21"
                },
                new ViewControlData
                {
                    ViewControlType = "Sotka1",
                    ViewControlName = "Sotka1",
                    Left = 150,
                    Top = 150,
                    IsActive = true,
                    ParentName = "Sotka2",
                    IPAddress = "10.10.12.11"
                },
                new ViewControlData
                {
                    ViewControlType = "RemoteDevice",
                    ViewControlName = "RemoteDevice",
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
                        controlDictionary[controlData.ViewControlType] = control; // Изменено на ViewControlType
                    }
                }

                // Затем устанавливаем родителей для всех контролов
                foreach (var controlData in loadedCanvasData.Controls)
                {
                    if (controlData.ParentName != null)
                    {
                        var control = controlDictionary[controlData.ViewControlType]; // Изменено на ViewControlType
                        var parent = controlDictionary[controlData.ParentName];
                        control.Parent = parent;
                        parent.AddChildren(control);
                    }
                }

                _isLinesDirty = true;
                UpdateLinesIfNeeded();
            }
            _infoTableViewModel?.UpdateStatistics();
        }

        private IViewControl CreateControl(ViewControlData controlData, Canvas canvas, IViewControl parent = null)
        {
            //MethodCallLogger.LogMethodCall(nameof(CreateControl));
            IViewControl control = null;
            switch (controlData.ViewControlType) // Изменено на ViewControlType
            {
                case "Sotka2":
                    control = new Sotka2(controlData.ViewControlName, controlData.IPAddress); // Используем ViewControlName и IPAddress
                    break;
                case "Sotka1":
                    control = new Sotka1(controlData.ViewControlName, controlData.IPAddress); // Используем ViewControlName и IPAddress
                    break;
                case "RemoteDevice":
                    // Создаем NodeParams для каждого RemoteDevice
                    var nodeParams = new ObservableCollection<NodeParam>();
                    foreach (var nodeParamData in controlData.NodeParams)
                    {
                        var nodeParam = new NodeParam(nodeParamData.ParamName, nodeParamData.ParamValue, nodeParamData.MeasureUnit);
                        nodeParams.Add(nodeParam);
                    }
                    control = new RemoteDevice(controlData.ViewControlName, nodeParams); // Используем ViewControlName
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
                    frameworkElement.MouseRightButtonDown += FrameworkElement_MouseRightButtonDown; // Добавляем обработчик клика правой кнопкой мыши
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

        private void FrameworkElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as IViewControl;
            if (control != null)
            {
                ToggleChildrenVisibility(control);
                _isLinesDirty = true;
                UpdateLinesIfNeeded(); // Вызываем UpdateLines после завершения всех рекурсивных вызовов
            }
        }

        // Нужен для скрытия/показа дочерних элементов при нажатии правой кнопки мыши
        private Dictionary<IViewControl, Visibility> _visibilityStates = new Dictionary<IViewControl, Visibility>();

        private void ToggleChildrenVisibility(IViewControl control)
        {
            //MethodCallLogger.LogMethodCall(nameof(ToggleChildrenVisibility));
            var controlElement = control as FrameworkElement;
            if (controlElement != null)
            {
                // Если родительский элемент скрыт, скрываем всех детей
                if (controlElement.Visibility == Visibility.Collapsed)
                {
                    foreach (var child in control.Elements)
                    {
                        var childElement = child as FrameworkElement;
                        if (childElement != null)
                        {
                            // Сохраняем текущую видимость перед скрытием
                            if (!_visibilityStates.ContainsKey(child))
                            {
                                _visibilityStates[child] = childElement.Visibility;
                            }
                            childElement.Visibility = Visibility.Collapsed;
                        }
                        ToggleChildrenVisibility(child);
                    }
                }
                else
                {
                    // Иначе переключаем видимость детей
                    foreach (var child in control.Elements)
                    {
                        var childElement = child as FrameworkElement;
                        if (childElement != null)
                        {
                            // Восстанавливаем видимость, если она была сохранена
                            if (_visibilityStates.ContainsKey(child))
                            {
                                childElement.Visibility = _visibilityStates[child];
                                _visibilityStates.Remove(child);
                            }
                            else
                            {
                                childElement.Visibility = childElement.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                            }
                        }
                        ToggleChildrenVisibility(child);
                    }
                }
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleFactor = 0.1;
            Point mousePosition = e.GetPosition(canvas);

            double newScaleX = canvasScaleTransform.ScaleX;
            double newScaleY = canvasScaleTransform.ScaleY;

            if (e.Delta > 0)
            {
                // Увеличиваем масштаб
                newScaleX += scaleFactor;
                newScaleY += scaleFactor;
            }
            else
            {
                // Уменьшаем масштаб, но не меньше чем 0.2 (уменьшение в 5 раз)
                if (canvasScaleTransform.ScaleX > 0.2 && canvasScaleTransform.ScaleY > 0.2)
                {
                    newScaleX -= scaleFactor;
                    newScaleY -= scaleFactor;
                }
            }

            // Проверяем, не выходят ли элементы за пределы канваса
            if (newScaleX > 0.2 && newScaleY > 0.2)
            {
                canvasScaleTransform.ScaleX = newScaleX;
                canvasScaleTransform.ScaleY = newScaleY;

                // Вычисляем смещение канваса относительно курсора мыши
                double offsetX = (mousePosition.X * newScaleX) - mousePosition.X;
                double offsetY = (mousePosition.Y * newScaleY) - mousePosition.Y;

                // Применяем смещение
                canvasTranslateTransform.X = -offsetX;
                canvasTranslateTransform.Y = -offsetY;
            }
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == canvas)
            {
                // Если кликнули по пустому месту на Canvas, выводим сообщение "Canvas"
                // MessageBox.Show("Canvas");
            }
            else if (element != null)
            {
                // Проверяем, был ли это двойной клик
                if ((DateTime.Now - _lastClickTime).TotalMilliseconds < 300) // 300 мс - интервал для двойного клика
                {
                    // Освобождаем захват мыши перед показом модального окна
                    if (element.IsMouseCaptured)
                    {
                        element.ReleaseMouseCapture();
                    }

                    // Сбрасываем флаг перетаскивания
                    _isDragging = false;

                    // Открываем модальное окно
                    if(element is RemoteDevice device)
                    {
                        new RegistryView(device.ViewControlName).ShowDialog();
                    }
                    else if (element is Sotka1 sotka)
                    {
                        new Sotka1Viewer(sotka.Clone()).ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(element.GetType().ToString());
                    }
                    
                }

                _lastClickTime = DateTime.Now; // Обновляем время последнего клика

                // Если кликнули по дочернему элементу, вызываем обработчик этого элемента
                element.RaiseEvent(e);
            }
        }
    }
}