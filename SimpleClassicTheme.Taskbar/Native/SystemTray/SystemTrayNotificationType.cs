namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    /// <remarks>
    /// These correspond to the NIM_* values described here
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyicona
    /// </remarks>
    internal enum SystemTrayNotificationType
	{
		IconAdded = 0,
		IconModified = 1,
		IconDeleted = 2,
		FocusTray = 3,
		SetVersion = 4
	}
}
