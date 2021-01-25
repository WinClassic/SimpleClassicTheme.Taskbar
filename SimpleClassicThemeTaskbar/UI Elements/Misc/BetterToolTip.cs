using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar.UIElements.Misc
{
    public class BetterToolTip : ToolTip
    {
        private const int TTS_BALLOON = 0x40;
        private const int TTS_CLOSE = 0x80;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                /*cp.Style = TTS_BALLOON /*| TTS_CLOSE*/;
                return cp;
            }
        }
    }
}
