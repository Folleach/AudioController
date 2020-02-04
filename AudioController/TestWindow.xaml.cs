using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AudioController
{
    // The cuckoo went again
    public partial class TestWindow : Window
    {
        private Stopwatch Time;
        private Action CloseCallback;

        private int LeftDownCount   = 0;
        private int LeftUpCount     = 0;
        private int MiddleDownCount = 0;
        private int MiddleUpCount   = 0;
        private int RightDownCount  = 0;
        private int RightUpCount    = 0;

        private static readonly string[] RandomText = new[]
        {
            "Button for your mouse",
            "Button loves you",
            "Click here",
            "If you want to click here... I...",
            "Don't click past"
        };

        public TestWindow(Action closeCallback)
        {
            InitializeComponent();
            UpdateMouseCounter();

            CloseCallback = closeCallback;
            MainWindow.Keyboard.Pressed += this.Keyboard_Pressed;
            this.Closing += this.TestWindow_Closing;

            ClickableSurfaceText.Content = RandomText[new Random().Next(RandomText.Length)];

            Time = Stopwatch.StartNew();
        }

        private void TestWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.Keyboard.Pressed -= this.Keyboard_Pressed;
            CloseCallback();
        }

        private void Keyboard_Pressed(System.Windows.Forms.Keys key)
        {
            KeyboardLog.Children.Add(new Label()
            {
                Content = $"{Time.ElapsedMilliseconds}\t|\t{key.ToString()}",
                Style = Application.Current.Resources["DefaultText"] as Style,
            });
        }

        private void KeyboardLog_Clear(object sender, RoutedEventArgs e)
        {
            KeyboardLog.Children.Clear();
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    LeftDownCount++;
                    break;
                case System.Windows.Input.MouseButton.Middle:
                    MiddleDownCount++;
                    break;
                case System.Windows.Input.MouseButton.Right:
                    RightDownCount++;
                    break;
            }
            UpdateMouseCounter();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case System.Windows.Input.MouseButton.Left:
                    LeftUpCount++;
                    break;
                case System.Windows.Input.MouseButton.Middle:
                    MiddleUpCount++;
                    break;
                case System.Windows.Input.MouseButton.Right:
                    RightUpCount++;
                    break;
            }
            UpdateMouseCounter();
        }

        private void UpdateMouseCounter()
        {
            LeftDown.Content = $"Left down: {LeftDownCount}";
            LeftUp.Content = $"Left up: {LeftUpCount}";
            MiddleDown.Content = $"Middle down: {MiddleDownCount}";
            MiddleUp.Content = $"Middle up: {MiddleUpCount}";
            RightDown.Content = $"Right down: {RightDownCount}";
            RightUp.Content = $"Right uMiddlep: {RightUpCount}";
        }
    }
}
