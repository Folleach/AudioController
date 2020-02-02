using AudioController.Controls;
using NAudio.CoreAudioApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AudioController
{
    public class Event
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                if (VisualItem != null)
                    VisualItem.ContentName.Content = name;
            }
        }
        public string DeviceID { get; private set; }
        public bool Active { get; set; }
        public float criticalValue;
        public float CriticalValue
        {
            get => criticalValue;
            set
            {
                if (value > 1)
                    criticalValue = 1;
                else if (value < 0)
                    criticalValue = 0;
                else
                    criticalValue = value;
            }
        }
        public Mode Mode { get; set; }
        public List<DeviceAction> Actions { get; set; }
        private MMDevice device;
        public MMDevice Device
        {
            get => device;
            set
            {
                if (value == null)
                    DeviceID = null;
                else
                    DeviceID = value.ID;
                device = value;
            }
        }
        public float CurrentValue;
        public EventItem VisualItem;

        public Event(MMDevice device)
        {
            Name = "Event name";
            Active = false;
            CriticalValue = 0.5f;
            Mode = Mode.Hold;
            Actions = new List<DeviceAction>();
            Device = device;
        }

        public void Update(MMDevice dd, bool isGlobalActive)
        {
            if (Device == null)
                return;
            CurrentValue = dd.AudioMeterInformation.MasterPeakValue;
            if (!Active || !isGlobalActive)
                return;
            //TODO: Works with value
        }
    }
}
