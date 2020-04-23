using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

using HWND = System.IntPtr;
using HDC = System.IntPtr;
using SimpleClassicThemeTaskbar.Cpp.CLI;

namespace SimpleClassicThemeTaskbar
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Interop d = new Interop();
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            d.SetWorkingArea(0, screen.Width, 0, screen.Height - 28);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Taskbar());
            d.SetWorkingArea(0, screen.Width, 0, screen.Height - 30);
        }
    }
}
