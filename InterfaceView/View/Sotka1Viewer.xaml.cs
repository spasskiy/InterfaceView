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
    /// Логика взаимодействия для Sotka1Viewer.xaml
    /// </summary>
    public partial class Sotka1Viewer : Window
    {
        public Sotka1 Sotka1 { get; set; }
        public Sotka1Viewer(Sotka1 sotka1)
        {
            Sotka1 = sotka1;
            DataContext = this;
            Owner = App.Current.MainWindow;
            InitializeComponent();
        }
    }
}
