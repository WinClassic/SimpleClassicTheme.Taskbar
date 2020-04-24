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
        static void Main(string[] args)
        {
            /*
             * I felt like this was needed:
             * 
             * ===========================================
             *         Simple Classic Theme Taskbar
             * ===========================================
             * An advanced taskbar replacement for Win10
             * 
             * The goal of this project is to replace the 
             * explorer taskbar and also replace my other
             * project SimpleClassicTheme.
             * 
             * ===========================================
             * DEFAULT PROGRAM FLOW GOAL:
             * 
             * (CHECK) Set new working area
             * Enable classic theme
             * Restart explorer
             * (CHECK) Show the tasbkbar
             * (CHECK) Reset working area
             * Disable classic theme
             * Restart explorer
             * (CHECK) Exit
             * 
             * ===========================================
             * COMMAND LINE GOALS (if there are any, 
             * arguments will be executed in following 
             * order and afterwards the program will exit)
             * --enable     | Enable classic theme
             * --disable    | Disable classic theme
             * --config     | Open classic theme config
             * 
             */
            if (args.Length > 0)
            {
                return;
            }
            Interop d = new Interop();
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            d.SetWorkingArea(0, screen.Width, 0, screen.Height - 28);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Taskbar());
            d.SetWorkingArea(0, screen.Width, 0, screen.Height - 30);
        }
    }
}
