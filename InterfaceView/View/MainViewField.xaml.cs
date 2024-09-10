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

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для MainViewField.xaml
    /// </summary>
    public partial class MainViewField : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;
        private List<Button> _buttons = new List<Button>();
        private List<Line> _lines = new List<Line>();

        public MainViewField()
        {
            InitializeComponent();
            AddButton("Датчик 1", 50, 50);
            AddButton("Датчик 2", 150, 150);
            AddButton("Датчик 3", 250, 250);
            AddButton("Датчик 4", 350, 350);
            DrawLines();
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

            button.PreviewMouseDown += Button_PreviewMouseDown;
            button.PreviewMouseMove += Button_PreviewMouseMove;
            button.PreviewMouseUp += Button_PreviewMouseUp;

            Canvas.SetLeft(button, left);
            Canvas.SetTop(button, top);

            canvas.Children.Add(button);
            _buttons.Add(button);
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            var draggableControl = sender as Button;
            _startPoint = e.GetPosition(draggableControl);
            draggableControl.CaptureMouse();
        }

        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var draggableControl = sender as Button;
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

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            var draggableControl = sender as Button;
            draggableControl.ReleaseMouseCapture();
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
                    Line line = new Line
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2
                    };

                    canvas.Children.Insert(0, line); // Добавляем линии в начало коллекции
                    _lines.Add(line);
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

                    Line line = _lines[lineIndex];
                    line.X1 = point1.X;
                    line.Y1 = point1.Y;
                    line.X2 = point2.X;
                    line.Y2 = point2.Y;

                    lineIndex++;
                }
            }
        }
    }
}