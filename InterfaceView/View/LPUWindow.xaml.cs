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
using System.Windows.Shapes;

namespace InterfaceView.View
{
    /// <summary>
    /// Логика взаимодействия для LPUWindow.xaml
    /// </summary>
    public partial class LPUWindow : Window
    {
        public LPUWindow(Sotka2 sotka)
        {
            Owner = App.Current.MainWindow;
            DataContext = sotka;
            InitializeComponent();
        }
    }
}
