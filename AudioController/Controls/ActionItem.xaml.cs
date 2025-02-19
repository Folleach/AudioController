﻿using System;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.InteropServices;

namespace AudioController.Controls
{
    public partial class ActionItem : UserControl
    {
        [DllImport("user32.dll")]
        private extern static IntPtr SetFocus(IntPtr hWnd);

        private Action<bool> UpdateUIContentCallback;
        private DeviceAction Action;

        public ActionItem(Action<bool> updateUICallback, DeviceAction action)
        {
            InitializeComponent();
            UpdateUIContentCallback = updateUICallback;
            Action = action;
            Combobox_Type.SelectedIndex = (int)action.Type;
            UpdateSelector();
        }

        private void DeleteAction(object sender, RoutedEventArgs e)
        {
            Action.Delete();
            UpdateUIContentCallback(true);
        }

        private void SelectedType(object sender, RoutedEventArgs e)
        {
            ComboBox combobox = sender as ComboBox;
            Action.Type = (ActionType)combobox.SelectedIndex;
            ShowSelector();
        }

        private void ShowSelector()
        {
            Selector_Keyboard.Visibility = Action.Type == ActionType.Keyboard ? Visibility.Visible : Visibility.Collapsed;
            Selector_Mouse.Visibility = Action.Type == ActionType.Mouse ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ChangeMouseValue(object sender, SelectionChangedEventArgs e)
        {
            // The cuckoo went
            Action.Value = int.Parse(((sender as ComboBox).SelectedItem as ComboBoxItem).Tag as string);
        }

        private void ChangeKeyboardValue(object sender, RoutedEventArgs e)
        {
            MainWindow.Keyboard.Pressed += ActionItem_Pressed;
        }

        public void UpdateSelector()
        {
            if (Action.Type == ActionType.Mouse)
            {
                int selected = 0;
                if (Action.Value == 8)
                    selected = 2;
                if (Action.Value == 32)
                    selected = 1;
                Selector_Mouse.SelectedIndex = selected;
                return;
            }
            if (Action.Type == ActionType.Keyboard)
            {
                Selector_Keyboard.Text = ((System.Windows.Forms.Keys)Action.Value).ToString();
                return;
            }
            ShowSelector();
        }

        private void ActionItem_Pressed(System.Windows.Forms.Keys key)
        {
            Action.Value = (int)key;
            Selector_Keyboard.Text = ((System.Windows.Forms.Keys)Action.Value).ToString();
            SetFocus(IntPtr.Zero);
            MainWindow.Keyboard.Pressed -= ActionItem_Pressed;
        }
    }
}
