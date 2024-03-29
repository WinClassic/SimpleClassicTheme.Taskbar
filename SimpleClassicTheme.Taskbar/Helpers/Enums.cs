﻿using System;

namespace SimpleClassicTheme.Taskbar.Helpers
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