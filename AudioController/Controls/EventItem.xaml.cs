using System;
using System.Windows.Controls;

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
        }

        private void Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectCallback(EventInfo);
        }
    }
}
