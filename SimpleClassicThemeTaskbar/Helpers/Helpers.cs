using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.Helpers
{
    internal class Helpers
    {
        public static IEnumerable<Taskbar> GetOpenTaskbars()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is Taskbar taskbar)
                {
                    yield return taskbar;
                }
            }
        }
    }
}