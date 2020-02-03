using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioController
{
    public class Keyboard
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_KEYDOWN = 0x0100;

        public event Action<Keys> Pressed;

        private delegate IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam);
        private KeyboardHookProc _proc;
        private IntPtr _hHook = IntPtr.Zero;

        public Keyboard()
        {
            _proc = HookProc;
            SetHook();
        }

        private void SetHook()
        {
            var hInstance = LoadLibrary("User32");
            _hHook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        private void Dispose()
        {
            UnHook();
        }

        private void UnHook()
        {
            UnhookWindowsHookEx(_hHook);
        }

        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WH_KEYDOWN)
                Pressed((Keys)Marshal.ReadInt32(lParam));
            return CallNextHookEx(_hHook, code, (int)wParam, lParam);
        }

        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public static void KeyboardDown(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }
    }
}
