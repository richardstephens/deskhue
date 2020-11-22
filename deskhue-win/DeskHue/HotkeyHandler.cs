using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace DeskHue
{
    class HotkeyHandler
    {
        public Action<Int32> adjustBrightness;
        
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
          [In] IntPtr hWnd,
          [In] int id,
          [In] uint fsModifiers,
          [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_DOWN = 9000;
        private const int HOTKEY_UP = 9001;


        public void OnSourceInitialized(EventArgs e, Window window)
        {
            var helper = new WindowInteropHelper(window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey(window);
        }

        public void OnClosed(EventArgs e, Window window)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey(window);
        }

        private void RegisterHotKey(Window window)
        {
            var helper = new WindowInteropHelper(window);
            const uint VK_F10 = 0x79;
            const uint VK_F13 = 0x7C;
            const uint VK_F14 = 0x7D;
            const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_DOWN, 0, VK_F13))
            {
                // handle error
            }
            if (!RegisterHotKey(helper.Handle, HOTKEY_UP, 0, VK_F14))
            {
                // handle error
            }
        }

        private void UnregisterHotKey(Window window)
        {
            var helper = new WindowInteropHelper(window);
            UnregisterHotKey(helper.Handle, HOTKEY_DOWN);
            UnregisterHotKey(helper.Handle, HOTKEY_UP);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_DOWN:
                            OnHotKeyDownPressed();
                            handled = true;
                            break;
                        case HOTKEY_UP:
                            OnHotKeyUpPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyDownPressed()
        {
            this.adjustBrightness(-25);
        }
        private void OnHotKeyUpPressed()
        {
            this.adjustBrightness(25);
        }
    }
}
