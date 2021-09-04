﻿using SimpleClassicTheme.Common.Providers;

using System.Collections.Generic;

namespace SimpleClassicTheme.Taskbar.Providers
{
    public class ActiveWindowProvider : FetchProvider<Window>
    {
        protected override IEnumerable<Window> Fetch()
        {
            return Taskbar.GetTaskbarWindows();
        }
    }
}