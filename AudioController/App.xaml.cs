using System;
using System.IO;
using System.Windows;

namespace AudioController
{
    public partial class App : Application
    {
        public static readonly string DataDirectory;

        static App()
        {
            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Folleach\\AudioController");
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
        }
    }
}
