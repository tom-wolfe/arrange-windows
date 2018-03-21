using System;
using System.Runtime.InteropServices;

namespace ArrangeWindows
{
    internal class PInvoke
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        public const uint SWP_SHOWWINDOW = 0x0040;
    }
}
