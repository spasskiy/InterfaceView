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
    /// Логика взаимодействия для ItemTreeViewControl.xaml
    /// </summary>
    public partial class ItemTreeViewControl : UserControl
    {
        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Canvas", typeof(Canvas), typeof(ItemTreeViewControl), new PropertyMetadata(null));

        public Canvas Canvas
        {
            get { return (Canvas)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }
        public ItemTreeViewControl()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
