using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GuiTestKit.Automation
{
    /// <summary>キャプチャ範囲の計算などで使う Win32 API。</summary>
    internal static class NativeMethods
    {
        private const uint GW_OWNER = 4;
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
            public Rectangle ToRectangle() { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X, Y;
        }

        [DllImport("user32.dll")] private static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")] private static extern IntPtr GetWindow(IntPtr hWnd, uint cmd);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
        [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr hWnd, out RECT rect);
        [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);
        [DllImport("user32.dll")] private static extern uint GetCaretBlinkTime();
        [DllImport("user32.dll")] private static extern bool SetCaretBlinkTime(uint milliseconds);
        [DllImport("user32.dll")] private static extern int GetSystemMetrics(int index);
        [DllImport("dwmapi.dll")] private static extern int DwmGetWindowAttribute(IntPtr hWnd, int attribute, out RECT value, int size);

        /// <summary>
        /// 高DPI(125%等)の環境でも、座標とキャプチャのピクセルがずれないようにする。
        /// テストプロセスの最初に1回呼ぶ。
        /// </summary>
        public static void EnableDpiAwareness()
        {
            try { SetProcessDPIAware(); } catch (EntryPointNotFoundException) { }
        }

        /// <summary>前面(アクティブ)ウィンドウが指定プロセスのものなら、そのハンドルを返す。</summary>
        public static IntPtr GetForegroundWindowOf(int processId)
        {
            var hWnd = GetForegroundWindow();
            if (hWnd == IntPtr.Zero) return IntPtr.Zero;
            uint pid;
            GetWindowThreadProcessId(hWnd, out pid);
            return pid == processId ? hWnd : IntPtr.Zero;
        }

        public static IntPtr GetOwner(IntPtr hWnd)
        {
            return GetWindow(hWnd, GW_OWNER);
        }

        /// <summary>ウィンドウの見た目どおりの枠（Windows 10 以降の透明な枠を含まない）。</summary>
        public static Rectangle GetVisibleWindowBounds(IntPtr hWnd)
        {
            RECT rect;
            if (DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT))) == 0)
            {
                return rect.ToRectangle();
            }
            GetWindowRect(hWnd, out rect);
            return rect.ToRectangle();
        }

        /// <summary>タイトルバーと枠を除いた中身部分の、画面上の位置とサイズ。</summary>
        public static Rectangle GetClientBoundsOnScreen(IntPtr hWnd)
        {
            RECT client;
            GetClientRect(hWnd, out client);
            var origin = new POINT();
            ClientToScreen(hWnd, ref origin);
            return new Rectangle(origin.X, origin.Y, client.Right - client.Left, client.Bottom - client.Top);
        }

        /// <summary>全モニタを合わせた範囲。</summary>
        public static Rectangle GetVirtualScreen()
        {
            return new Rectangle(
                GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN),
                GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN));
        }

        public static uint GetCaretBlink() { return GetCaretBlinkTime(); }

        public static void SetCaretBlink(uint milliseconds) { SetCaretBlinkTime(milliseconds); }
    }
}
