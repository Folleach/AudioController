using AudioController.Controls;
using System.Windows.Forms;

namespace AudioController
{
    public class DeviceAction
    {
        public ActionType Type = ActionType.Mouse;
        public int Value;
        public ActionItem VisualItem;
        public readonly Event Owner;

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
            switch (Type)
            {
                case ActionType.Mouse:
                    Mouse.MouseEvent((LLMouseEvent)(Value << 1));
                    return;
                case ActionType.Keyboard:
                    // Nothing to action here...
                    return;
            }
        }
    }
}
