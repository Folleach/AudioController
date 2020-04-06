using AudioController.Controls;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AudioController
{
    [Serializable]
    public class Event
    {
        [XmlIgnore]
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
        public string DeviceID { get; set; }
        public bool Active { get; set; }
        public float criticalValue;
        [XmlIgnore]
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
        [XmlIgnore] public float OldValue;
        [XmlIgnore] public LLMouseEvent OldEvent;
        [XmlIgnore] public float CurrentValue;
        [XmlIgnore] public EventItem VisualItem;

        public Event()
        {
        }

        public Event(string deviceID)
        {
            Name = "Event name";
            Active = false;
            CriticalValue = 0.5f;
            Mode = Mode.Hold;
            Actions = new List<DeviceAction>();
            DeviceID = deviceID;
        }

        public void Update(MMDevice dd, bool isGlobalActive)
        {
            if (DeviceID == null || dd == null)
                return;
            CurrentValue = dd.AudioMeterInformation.MasterPeakValue;
            if (!Active || !isGlobalActive)
                return;
            Act(CurrentValue);
        }

        private void Act(float value)
        {
            if (Mode == Mode.Hold)
            {
                if (value >= CriticalValue && OldValue < CriticalValue)
                    foreach (var item in Actions)
                        item.Down();
                else if (value < CriticalValue && OldValue >= CriticalValue)
                    foreach (var item in Actions)
                        item.Up();
            }
            else if (Mode == Mode.Switch)
            {
                if (value >= CriticalValue && OldValue < CriticalValue)
                {
                    foreach (var item in Actions)
                    {
                        if (item.IsOldDown)
                            item.Up();
                        else
                            item.Down();
                    }
                }
            }
            OldValue = value;
        }

        public void AddAction(DeviceAction action, Action<bool> updateUIContentCallback)
        {
            Actions.Add(action);
            action.VisualItem = new ActionItem(updateUIContentCallback, action);
        }

        public void UpdateLinks(Action<bool> updateUIContentCallback)
        {
            foreach (var action in Actions)
            {
                action.VisualItem = new ActionItem(updateUIContentCallback, action);
                action.Owner = this;
                // action.VisualItem.UpdateSelector();
            }
        }
    }
}
