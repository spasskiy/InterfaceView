using InterfaceView.View.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


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
        private List<Button> _buttons = new List<Button>();
        private List<Tuple<Line, Line>> _lines = new List<Tuple<Line, Line>>();

        public MainViewField()
        {
            InitializeComponent();
            AddButton("Датчик 1", 50, 50);
            AddButton("Датчик 2", 150, 150);
            AddButton("Датчик 3", 250, 250);
            AddButton("Датчик 4", 350, 350);
            AddButton("Датчик 5", 450, 450);
            AddButton("Датчик 6", 550, 550);
            DrawLines();

            var sotka = new Sotka2("ЛПУ");
            sotka.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            sotka.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            sotka.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
            canvas.Children.Add(sotka);

            var sotka1 = new Sotka1("ЛЭС1");
            sotka1.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            sotka1.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            sotka1.PreviewMouseUp += FrameworkElement_PreviewMouseUp;
            canvas.Children.Add(sotka1);
            List<IViewControl> views = new List<IViewControl>();
            foreach (var view in canvas.Children)
            {
                if (view is IViewControl) views.Add((IViewControl)view);
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

                UpdateLines();
            }
        }

        private void FrameworkElement_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var draggableControl = sender as FrameworkElement;
            draggableControl.ReleaseMouseCapture();
        }


        private void AddButton(string content, double left, double top)
        {
            Button button = new Button
            {
                Content = content,
                Width = 100,
                Height = 30,
                Background = Brushes.LightBlue
            };

            button.PreviewMouseDown += FrameworkElement_PreviewMouseDown;
            button.PreviewMouseMove += FrameworkElement_PreviewMouseMove;
            button.PreviewMouseUp += FrameworkElement_PreviewMouseUp;

            Canvas.SetLeft(button, left);
            Canvas.SetTop(button, top);

            canvas.Children.Add(button);
            _buttons.Add(button);
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var button in _buttons)
            {
                double newLeft = Canvas.GetLeft(button);
                double newTop = Canvas.GetTop(button);

                // Ограничение перемещения внутри Canvas с отступом
                if (newLeft < 0) newLeft = 0;
                if (newTop < 0) newTop = 0;
                if (newLeft + button.ActualWidth > canvas.ActualWidth)
                    newLeft = canvas.ActualWidth - button.ActualWidth;
                if (newTop + button.ActualHeight > canvas.ActualHeight - 1) // Отступ 1 пиксель
                    newTop = canvas.ActualHeight - button.ActualHeight - 3;

                Canvas.SetLeft(button, newLeft);
                Canvas.SetTop(button, newTop);
            }
            UpdateLines();
        }

        private void DrawLines()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                for (int j = i + 1; j < _buttons.Count; j++)
                {
                    Line verticalLine = new Line
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 3
                    };

                    Line horizontalLine = new Line
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 3
                    };

                    canvas.Children.Insert(0, verticalLine); // Добавляем линии в начало коллекции
                    canvas.Children.Insert(0, horizontalLine); // Добавляем линии в начало коллекции
                    _lines.Add(new Tuple<Line, Line>(verticalLine, horizontalLine));
                }
            }
            UpdateLines();
        }

        private void UpdateLines()
        {
            int lineIndex = 0;
            for (int i = 0; i < _buttons.Count; i++)
            {
                for (int j = i + 1; j < _buttons.Count; j++)
                {
                    Button button1 = _buttons[i];
                    Button button2 = _buttons[j];

                    Point point1 = new Point(Canvas.GetLeft(button1) + button1.ActualWidth / 2, Canvas.GetTop(button1) + button1.ActualHeight / 2);
                    Point point2 = new Point(Canvas.GetLeft(button2) + button2.ActualWidth / 2, Canvas.GetTop(button2) + button2.ActualHeight / 2);

                    Tuple<Line, Line> lines = _lines[lineIndex];
                    Line verticalLine = lines.Item1;
                    Line horizontalLine = lines.Item2;

                    // Вертикальная линия
                    verticalLine.X1 = point1.X;
                    verticalLine.Y1 = point1.Y;
                    verticalLine.X2 = point1.X;
                    verticalLine.Y2 = point2.Y;

                    // Горизонтальная линия
                    horizontalLine.X1 = point1.X;
                    horizontalLine.Y1 = point2.Y;
                    horizontalLine.X2 = point2.X;
                    horizontalLine.Y2 = point2.Y;

                    lineIndex++;
                }
            }
        }
    }
}