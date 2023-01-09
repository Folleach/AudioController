using AudioController.Controls;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;

namespace AudioController
{
    public partial class MainWindow : Window
    {
        Thread updater;
        internal static Keyboard Keyboard;
        bool needToUpdateDevices;
        Mutex eventsMutext;
        int countOpenedTestWindows = 0;
        WaveIn waveIn;

        public bool GlobalActive = false;
        public int GlobalDelay = 10;

        public static List<MMDevice> Devices = new List<MMDevice>();
        public static List<Event> Events = new List<Event>();

        public Event SelectedEvent;
        
        static MainWindow()
        {
            Keyboard = new Keyboard();
        }

        public MainWindow()
        {
            InitializeComponent();
            eventsMutext = new Mutex();
            EventsContainer.Children.Clear();
            ContentStack.Visibility = Visibility.Collapsed;

            Keyboard.Pressed += this.Keyboard_Pressed;

            UpdateDevice(null, null);
            StartUpdater();

            LoadData();

            UpdateUIGlobal();
            UpdateUIAll();

            waveIn = new WaveIn();
            waveIn.StartRecording();
        }

        private void LoadData()
        {
            if (!DataFile.ContainsSavedData)
                return;
            try
            {
                DataFile data = DataFile.Load();
                GlobalDelay = data.Delay;
                foreach (var @event in data.Events)
                {
                    @event.UpdateLinks(UpdateUIContent);
                    AddEvent(@event);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void UpdateUIGlobal()
        {
            Global_TTU.Text = GlobalDelay.ToString();
            Global_Active.Content = GlobalActive ? "YES" : "NO";
        }

        private void UpdateUIAll()
        {
            if (IsActive || countOpenedTestWindows > 0)
            {
                foreach (var item in Events)
                    item.VisualItem.UpdateUI();
                UpdateUIContent(false);
            }
        }

        private void UpdateUIContent(bool force)
        {
            if (SelectedEvent == null)
                return;
            Content_DeviceValue.Value = SelectedEvent.CurrentValue;
            if (force)
            {
                Content_Name.Text = SelectedEvent.Name;
                Content_State.Content = $"Current state: {(SelectedEvent.Active ? "ON" : "OFF")}";
                Content_CriticalValue.Value = SelectedEvent.CriticalValue;
                Content_ModeHold.IsChecked = SelectedEvent.Mode == Mode.Hold;
                Content_ModeSwitch.IsChecked = SelectedEvent.Mode == Mode.Switch;
                Content_ActionsContainer.Children.Clear();
                foreach (DeviceAction action in SelectedEvent.Actions)
                    Content_ActionsContainer.Children.Add(action.VisualItem);
            }
        }

        private void StartUpdater()
        {
            updater = new Thread(Updater);
            updater.Name = "Background updater";
            updater.IsBackground = true;
            updater.Start();
        }

        private void Updater()
        {
            Dictionary<string, MMDevice> devices = new Dictionary<string, MMDevice>();
            while (true)
            {
                Thread.Sleep(GlobalDelay);
                if (needToUpdateDevices)
                {
                    devices.Clear();
                    foreach (var device in GetDevices())
                        devices.Add(device.ID, device);
                    needToUpdateDevices = false;
                }
                eventsMutext.WaitOne();
                foreach (Event item in Events)
                    item.Update(item.DeviceID != null && devices.ContainsKey(item.DeviceID) ? devices[item.DeviceID] : null, GlobalActive);
                eventsMutext.ReleaseMutex();
                Dispatcher.Invoke(() =>
                {
                    UpdateUIAll();
                });
            }
        }

        private void Keyboard_Pressed(Keys obj)
        {
            if (obj == Keys.R)
            {
                GlobalActive = !GlobalActive;
                UpdateUIGlobal();
            }
        }

        private void UpdateDevice(object sender, RoutedEventArgs e)
        {
            Devices.Clear();
            Content_Devices.Children.Clear();
            MMDevice[] devices = GetDevices();
            Devices.AddRange(devices);
            Content_Devices.Children.Add(new DeviceItem(null, "Not selected", SelectDevice));
            foreach (var device in Devices)
                Content_Devices.Children.Add(new DeviceItem(device.ID, device.GetCustomName(), SelectDevice));
            needToUpdateDevices = true;
            Global_DeviceAvailableText.Content = $"{devices.Length} devices available";
        }

        private MMDevice[] GetDevices()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            return enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).ToArray();
        }

        private void OpenTestWindow(object sender, RoutedEventArgs e)
        {
            TestWindow window = new TestWindow(() =>
            {
                countOpenedTestWindows--;
            });
            countOpenedTestWindows++;
            window.Show();
        }

        private void AddEvent(object sender, RoutedEventArgs e)
        {
            AddEvent(new Event(null));
        }

        private void AddEvent(Event eventInfo)
        {
            eventsMutext.WaitOne();
            Events.Add(eventInfo);
            eventsMutext.ReleaseMutex();
            EventItem item = new EventItem(eventInfo, SelectEvent);
            EventsContainer.Children.Add(item);
        }

        private void SelectDevice(string id)
        {
            if (SelectedEvent == null)
                return;
            SelectedEvent.DeviceID = id;
            SetDevicesListHighlight(id);
        }

        private void SelectEvent(Event obj)
        {
            SelectedEvent = obj;
            ContentStack.Visibility = Visibility.Visible;
            SetDevicesListHighlight(obj.DeviceID);
            UpdateUIContent(true);
        }

        private void SetDevicesListHighlight(string idToHighlight)
        {
            foreach (var item in Content_Devices.Children)
            {
                DeviceItem deviceItem = item as DeviceItem;
                if (deviceItem == null)
                    continue;
                deviceItem.SetHighlight(idToHighlight == deviceItem.DeviceID);
            }
        }

        private void Content_ChangeCriticalValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (SelectedEvent == null || slider == null)
                return;
            SelectedEvent.CriticalValue = (float)slider.Value;
        }

        private void DeleteSelectedEvent(object sender, RoutedEventArgs e)
        {
            EventsContainer.Children.Remove(GetUIPreviewOfSelectedEvent());
            eventsMutext.WaitOne();
            Events.Remove(SelectedEvent);
            eventsMutext.ReleaseMutex();
            SelectedEvent = null;
            ContentStack.Visibility = Visibility.Hidden;
        }

        private void ChangeNameOfSelectedEvent(object sender, RoutedEventArgs e)
        {
            EventItem item = GetUIPreviewOfSelectedEvent() as EventItem;
            item.EventInfo.Name = (sender as System.Windows.Controls.TextBox).Text;
        }

        private UIElement GetUIPreviewOfSelectedEvent()
        {
            foreach (UIElement item in EventsContainer.Children)
            {
                if (item == SelectedEvent.VisualItem)
                    return item;
            }
            return null;
        }

        private void ChangeGlobalTTU(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (int.TryParse(tb.Text, out int delay) && delay >= 0)
            {
                GlobalDelay = delay;
                string color = "RedColor";
                if (GlobalDelay > 1 && GlobalDelay <= 16)
                    color = "GreenColor";
                else if (GlobalDelay > 0 && GlobalDelay <= 72)
                    color = "OrangeColor";
                Global_TTU.Foreground = (SolidColorBrush)System.Windows.Application.Current.Resources[color];
            }
            UpdateUIGlobal();
        }

        private void AddAction(object sender, RoutedEventArgs e)
        {
            if (SelectedEvent == null)
                return;
            SelectedEvent.AddAction(new DeviceAction(SelectedEvent), UpdateUIContent);
            UpdateUIContent(true);
        }

        private void ChangeStateOfSelectedEvent(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedEvent == null)
                return;
            SelectedEvent.Active = !SelectedEvent.Active;
            UpdateUIContent(true);
        }

        private void ChangeModeOfSelectionEvent(object sender, RoutedEventArgs e)
        {
            if (SelectedEvent == null)
                return;
            SelectedEvent.Mode = Content_ModeHold.IsChecked == true ? Mode.Hold : Mode.Switch;
            UpdateUIContent(true);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            waveIn.StopRecording();
            DataFile data = new DataFile();
            data.Delay = GlobalDelay;
            data.Events = Events;
            DataFile.Save(data);
        }
    }
}
