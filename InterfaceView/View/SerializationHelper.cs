using System.IO;
using System.Xml.Serialization;

namespace InterfaceView.View
{
    public static class SerializationHelper
    {
        public static void SerializeToFile<T>(T data, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, data);
            }
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}