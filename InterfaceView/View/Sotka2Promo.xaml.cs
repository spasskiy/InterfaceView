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
    /// Логика взаимодействия для Sotka2Promo.xaml
    /// </summary>
    public partial class Sotka2Promo : Window
    {
        public Sotka2Promo()
        {
            InitializeComponent();
        }
    }

    public class DataItem
    {
        public string? Object { get; set; }
        public string? Date { get; set; }
        public double Value { get; set; }
    }
}
