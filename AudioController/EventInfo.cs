using System.Collections.Generic;

namespace AudioController
{
    public class EventInfo
    {
        public string Name { get; set; }
        public string DeviceID { get; set; }
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
    }
}
