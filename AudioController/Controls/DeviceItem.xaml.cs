using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AudioController.Controls
{
    public partial class DeviceItem : UserControl
    {
        public string DeviceID;
        public Action<string> SelectCallback;

        public DeviceItem(string id, string visibilityName, Action<string> selectCallback)
        {
            InitializeComponent();
            DeviceID = id;
            SelectCallback = selectCallback;
            DeviceName.Content = visibilityName;
        }

        private void Click(object sender, MouseButtonEventArgs e)
        {
            SelectCallback(DeviceID);
        }

        public void SetHighlight(bool highlight)
        {
            DeviceName.Background = (SolidColorBrush)(highlight ? Application.Current.Resources["AccentColor"] : Application.Current.Resources["TransparentColor"]);
        }
    }
}
