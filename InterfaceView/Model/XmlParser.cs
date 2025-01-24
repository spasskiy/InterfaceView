using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterfaceView.Model
{
    public class XmlParser
    {
        public ObservableCollection<CellData> ParseXmlToCellDataCollection(string filePath)
        {
            // Загружаем XML-файл
            XDocument xmlDoc = XDocument.Load(filePath);

            // Получаем все элементы <CellData>
            var cellDataElements = xmlDoc.Descendants("CellData");

            // Создаем коллекцию для хранения данных
            var cellDataCollection = new ObservableCollection<CellData>();

            // Проходим по каждому элементу <CellData>
            foreach (var cellDataElement in cellDataElements)
            {
                // Создаем объект CellDataXml
                var cellDataXml = new CellDataXml
                {
                    Category = (string)cellDataElement.Element("Category"),
                    Address = (int)cellDataElement.Element("Address"),
                    NumberReg = (string)cellDataElement.Element("NumberReg"),
                    NameDevice = (string)cellDataElement.Element("NameDevice"),
                    Name = (string)cellDataElement.Element("Name"),
                    Format = (string)cellDataElement.Element("Format"),
                    Type = (string)cellDataElement.Element("Type"),
                    Uelement = new Uelement
                    {
                        Value = (int)cellDataElement.Element("Uelement")?.Element("Value")
                    }
                };

                // Преобразуем CellDataXml в CellData
                var cellData = new CellData
                {
                    Address = cellDataXml.Address,
                    NameDevice = cellDataXml.NameDevice,
                    Name = cellDataXml.Name,
                    Type = cellDataXml.Type,
                    Format = cellDataXml.Format,
                    Value = cellDataXml.Uelement.Value // Заполняем Value из Uelement.Value
                };

                // Добавляем объект в коллекцию
                cellDataCollection.Add(cellData);
            }

            return cellDataCollection;
        }
    }
}
