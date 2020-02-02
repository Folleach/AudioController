﻿using AudioController.Controls;
using NAudio.CoreAudioApi;
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
        Keyboard keyboard;
        bool needToUpdateDevices;

        public bool GlobalActive = false;
        public int GlobalDelay = 10;

        public static List<MMDevice> Devices = new List<MMDevice>();
        public static List<Event> Events = new List<Event>();

        public Event SelectedEvent;
        
        public MainWindow()
        {
            InitializeComponent();
            EventsContainer.Children.Clear();
            ContentStack.Visibility = Visibility.Collapsed;

            keyboard = new Keyboard();
            keyboard.Pressed += this.Keyboard_Pressed;

            UpdateDevice(null, null);
            StartUpdater();

            UpdateUIGlobal();
        }

        private void UpdateUIGlobal()
        {
            Global_TTU.Text = GlobalDelay.ToString();
            Global_Active.Content = GlobalActive ? "YES" : "NO";
        }

        private void UpdateUIAll()
        {
            if (IsActive)
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
                foreach (Event e in Events)
                    e.Update(e.DeviceID != null ? devices[e.DeviceID] : null, GlobalActive);
                Dispatcher.Invoke(() =>
                {
                    UpdateUIAll();
                });
            }
        }

        private void Keyboard_Pressed(Keys obj)
        {
            if (obj == Keys.F6)
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
            //TODO: Addded window
        }

        private void AddEvent(object sender, RoutedEventArgs e)
        {
            AddEvent(new Event(null));
        }

        private void AddEvent(Event eventInfo)
        {
            Events.Add(eventInfo);
            EventItem item = new EventItem(eventInfo, SelectEvent);
            EventsContainer.Children.Add(item);
        }

        private void SelectDevice(string id)
        {
            if (SelectedEvent == null)
                return;
            SelectedEvent.Device = Devices.FirstOrDefault(x => x.ID == id);
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
            Events.Remove(SelectedEvent);
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
                if (GlobalDelay > 0 && GlobalDelay <= 16)
                    color = "GreenColor";
                else if (GlobalDelay > 0 && GlobalDelay <= 72)
                    color = "OrangeColor";
                Global_TTU.Foreground = (SolidColorBrush)System.Windows.Application.Current.Resources[color];
            }
            UpdateUIGlobal();
        }
    }
}
