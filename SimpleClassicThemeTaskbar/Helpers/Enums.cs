using System;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public enum ExitMenuItemCondition
    {
        Always = 0,
        RequireShortcut = 1,
    }

    [Flags]
    public enum MouseState
    {
        Normal = 0,
        Hover,
        Pressed,
    }

    public enum ProgramGroupCheck
    {
        None = 0,
        Process = 1,
        FileNameAndPath = 2,
        ModuleName = 3
    }

    public enum StartButtonAppearance
    {
        Default = 0,
        CustomIcon = 1,
        CustomButton = 2
    }
}