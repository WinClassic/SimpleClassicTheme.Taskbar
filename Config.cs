// Boolean values
scttSubKey.SetValue("EnableSystemTrayHover", EnableSystemTrayHover.ToString(), RegistryValueKind.String);
scttSubKey.SetValue("EnableSystemTrayColorChange", EnableSystemTrayColorChange.ToString(), RegistryValueKind.String);
scttSubKey.SetValue("ShowTaskbarOnAllDesktops", ShowTaskbarOnAllDesktops.ToString(), RegistryValueKind.String);
scttSubKey.SetValue("EnableQuickLaunch", EnableQuickLaunch.ToString(), RegistryValueKind.String);

// Integer values
scttSubKey.SetValue("TaskbarProgramWidth", TaskbarProgramWidth, RegistryValueKind.DWord);
scttSubKey.SetValue("SpaceBetweenTrayIcons", SpaceBetweenTrayIcons, RegistryValueKind.DWord);
scttSubKey.SetValue("SpaceBetweenTaskbarIcons", SpaceBetweenTaskbarIcons, RegistryValueKind.DWord);
scttSubKey.SetValue("SpaceBetweenQuickLaunchIcons", SpaceBetweenQuickLaunchIcons, RegistryValueKind.DWord);

// Enum values
scttSubKey.SetValue("ProgramGroupCheck", ProgramGroupCheck, RegistryValueKind.DWord);
scttSubKey.SetValue("StartButtonAppearance", StartButtonAppearance, RegistryValueKind.DWord);

// String values
scttSubKey.SetValue("StartButtonImage", StartButtonImage);
scttSubKey.SetValue("StartButtonIconImage", StartButtonIconImage);
scttSubKey.SetValue("QuickLaunchOrder", QuickLaunchOrder);
scttSubKey.SetValue("TaskbarProgramFilter", TaskbarProgramFilter);
scttSubKey.SetValue("RendererPath", RendererPath);

// Boolean values
_ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayHover", "True"), out EnableSystemTrayHover);
_ = bool.TryParse((string)scttSubKey.GetValue("EnableSystemTrayColorChange", "True"), out EnableSystemTrayColorChange);
_ = bool.TryParse((string)scttSubKey.GetValue("ShowTaskbarOnAllDesktops", "True"), out ShowTaskbarOnAllDesktops);
_ = bool.TryParse((string)scttSubKey.GetValue("EnableQuickLaunch", "True"), out EnableQuickLaunch);

// Integer values
TaskbarProgramWidth = (int)scttSubKey.GetValue("TaskbarProgramWidth", 160);
SpaceBetweenTrayIcons = (int)scttSubKey.GetValue("SpaceBetweenTrayIcons", 2);
SpaceBetweenTaskbarIcons = (int)scttSubKey.GetValue("SpaceBetweenTaskbarIcons", 2);
SpaceBetweenQuickLaunchIcons = (int)scttSubKey.GetValue("SpaceBetweenQuickLaunchIcons", 2);

// String values
StartButtonImage = (string)scttSubKey.GetValue("StartButtonImage", "");
StartButtonIconImage = (string)scttSubKey.GetValue("StartButtonIconImage", "");
QuickLaunchOrder = (string)scttSubKey.GetValue("QuickLaunchOrder", "");
TaskbarProgramFilter = (string)scttSubKey.GetValue("TaskbarProgramFilter", "");
RendererPath = (string)scttSubKey.GetValue("RendererPath", "Internal/Classic");

// Enum values
ProgramGroupCheck = (ProgramGroupCheck)scttSubKey.GetValue("ProgramGroupCheck", ProgramGroupCheck.FileNameAndPath);
StartButtonAppearance = (StartButtonAppearance)scttSubKey.GetValue("StartButtonAppearance", StartButtonAppearance.Default);