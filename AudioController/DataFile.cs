using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AudioController
{
    [Serializable]
    public class DataFile
    {
        public int Delay;
        public List<Event> Events;

        public static void Save(DataFile data)
        {
            XmlSerializer xml = new XmlSerializer(typeof(DataFile));
            MemoryStream writer = new MemoryStream();
            xml.Serialize(writer, data);
            File.WriteAllBytes(FilePath, writer.GetBuffer());
        }

        public static DataFile Load()
        {
            XmlSerializer xml = new XmlSerializer(typeof(DataFile));
            return (DataFile)xml.Deserialize(new MemoryStream(File.ReadAllBytes(FilePath)));
        }

        [XmlIgnore]
        public static bool ContainsSavedData => File.Exists(FilePath);

        [XmlIgnore]
        private static string FilePath => Path.Combine(App.DataDirectory, "save.xml");
    }
}
