namespace SimpleClassicThemeTaskbar.Helpers
{
    public enum ExitMenuItemCondition
    {
        Always = 0,
        RequireShortcut = 1,
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