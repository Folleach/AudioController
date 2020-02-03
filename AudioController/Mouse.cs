using System.Runtime.InteropServices;

namespace AudioController
{
    public static class Mouse
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var point = GetCursorPos(out currentMousePoint);
            if (!point)
                currentMousePoint = new MousePoint(0, 0);
            return currentMousePoint;
        }

        public static void MouseEvent(LLMouseEvent value)
        {
            MousePoint position = GetCursorPosition();
            mouse_event((int)value, position.X, position.Y, 0, 0);
        }
    }
}
