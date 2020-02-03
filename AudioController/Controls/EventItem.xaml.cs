using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioController.Controls
{
    public partial class EventItem : UserControl
    {
        public Event EventInfo;
        public Action<Event> SelectCallback;

        public EventItem(Event eventInfo, Action<Event> selectCallback)
        {
            InitializeComponent();
            EventInfo = eventInfo;
            ContentName.Content = eventInfo.Name;
            EventInfo.VisualItem = this;
            SelectCallback = selectCallback;
        }

        public void UpdateUI()
        {
            PreviewValue.Value = EventInfo.CurrentValue;
            ContentRhombusActive.Fill = (SolidColorBrush)(EventInfo.Active == true ? Application.Current.Resources["GreenColor"] : Application.Current.Resources["RedColor"]);
        }

        private void Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectCallback(EventInfo);
        }

        private void ChangeActiveState(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EventInfo.Active = !EventInfo.Active;
            UpdateUI();
        }
    }
}
