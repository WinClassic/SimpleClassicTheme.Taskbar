using SimpleClassicTheme.Common.Providers;
using SimpleClassicTheme.Taskbar.Native;

using System;

namespace SimpleClassicTheme.Taskbar.Providers
{
    internal class ShellHookWindowProvider : Provider<Window>
    {
        public ShellHook ShellHook { get; private set; }

        public ShellHookWindowProvider(ShellHook shellHook)
        {
            ShellHook = shellHook ?? throw new ArgumentNullException(nameof(shellHook));
            ShellHook.WindowCreated += ShellHook_WindowCreated;
            ShellHook.WindowDestroyed += ShellHook_WindowDestroyed;
        }

        private void ShellHook_WindowCreated(object sender, IntPtr e)
        {
            OnItemAdded(new(e));
        }

        private void ShellHook_WindowDestroyed(object sender, IntPtr e)
        {
            OnItemRemoved(new(e));
        }
    }
}
