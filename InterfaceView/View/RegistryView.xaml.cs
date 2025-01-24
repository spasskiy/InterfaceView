using InterfaceView.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для RegistryView.xaml
    /// </summary>
    public partial class RegistryView : Window
    {
        public string DeviceName { get; set; }
        public ObservableCollection<CellData> CellDataCollection { get; set; }        
        public RegistryView(string deviceName)
        {
            DeviceName = deviceName;
            DataContext = this;
            FillTemplateDataGrid();
            InitializeComponent();
        }

        public void FillTemplateDataGrid()
        {
            // Создаем экземпляр парсера
            var xmlParser = new XmlParser();

            // Путь к XML-файлу
            string filePath = @"Content\Demo.xml";

            // Заполняем коллекцию данными из XML
            CellDataCollection = xmlParser.ParseXmlToCellDataCollection(filePath);
            UpdateCellDataValues(); //Для демонстрации записываем случайные значения в Value

        }
        public void UpdateCellDataValues()
        {
            Random random = new Random();

            foreach (var cellData in CellDataCollection)
            {
                // Генерируем случайное число от 0 до 1
                double probability = random.NextDouble();

                // Если вероятность меньше или равна 0.5, выбираем число из отрезка 1-100
                if (probability <= 0.5)
                {
                    cellData.Value = random.Next(1, 101); // 1-100
                }
                else
                {
                    // Иначе выбираем число из отрезка 101-10000
                    cellData.Value = random.Next(101, 10001); // 101-10000
                }
            }
        }
    }
}
