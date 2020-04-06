using AudioController.Controls;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AudioController
{
    [Serializable]
    public class DeviceAction
    {
        public ActionType Type = ActionType.Mouse;
        public int Value;
        public bool IsOldDown;
        [XmlIgnore] public ActionItem VisualItem;
        [XmlIgnore] public Event Owner;

        public DeviceAction()
        {
        }

        public DeviceAction(Event owner)
        {
            Owner = owner;
        }

        public void Delete()
        {
            Owner.Actions.Remove(this);
        }

        public void Down()
        {
            IsOldDown = true;
            switch (Type)
            {
                case ActionType.Mouse:
                    Mouse.MouseEvent((LLMouseEvent)Value);
                    return;
                case ActionType.Keyboard:
                    Keyboard.KeyboardDown((Keys)Value);
                    return;
            }
        }

        public void Up()
        {
            IsOldDown = false;
            switch (Type)
            {
                case ActionType.Mouse:
                    Mouse.MouseEvent((LLMouseEvent)(Value << 1));
                    return;
                case ActionType.Keyboard:
                    Keyboard.KeyboardUp((Keys)Value);
                    return;
            }
        }
    }
}
