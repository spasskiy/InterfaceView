using InterfaceView.Model;
using InterfaceView.View;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfaceView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _dragStartPoint;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ICommand SaveCommand => new RelayCommand(SaveToXml);

        private void SaveToXml()
        {
            var mainViewField = FindName("MainViewField") as MainViewField;
            if (mainViewField != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                    DefaultExt = "xml",
                    FileName = "canvasData.xml"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    mainViewField.SaveToXml(saveFileDialog.FileName);
                }
            }
        }
    }
}