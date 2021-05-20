using SimpleClassicThemeTaskbar.Helpers.NativeMethods;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SimpleClassicThemeTaskbar.Helpers
{
	// These correspond to the NIM_* values described here
	// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/nf-shellapi-shell_notifyicona
	internal enum SystemTrayNotificationType
	{
		IconAdded = 0,
		IconModified = 1,
		IconDeleted = 2,
		FocusTray = 3,
		SetVersion = 4
	}

	[Flags]
	internal enum SystemTrayNotificationFlags : int
	{
		None = 0,
		CallbackMessageValid = 1,
		IconHandleValid = 2,
		ToolTipValid = 4,
		StateValid = 8,
		InfoBalloonValid = 16,
		GuidValid = 32,
		Realtime = 64,
		ShowToolTip = 128
	}

	[Flags]
	internal enum SystemTrayIconState : int
	{
		None = 0,
		Hidden = 1,
		IconHandleShared = 2
	}

	[Flags]
	internal enum SystemTrayIconInfoBalloonFlags : int
	{
		NoIcon = 0,
		InfoIcon = 1,
		WarningIcon = 2,
		ErrorIcon = 3,
		UserIcon = 4,
		NoSound = 0x10,
		LargeIcon = 0x20,
		RespectQuietTime = 0x80
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal class SystemTrayNotificationData
	{
		public int Size/* = Marshal.SizeOf(typeof(SystemTrayNotificationData))*/;
		private int windowHandle;
		public uint Id;
		public SystemTrayNotificationFlags Flags;
		public uint CallbackMessage;
		private int iconHandle;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
		public string ToolTip;
		public SystemTrayIconState State;
		public SystemTrayIconState StateMask;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
		public string InfoBalloonText;
		private int TimeoutOrVersion;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
		public string InfoBalloonTitle;
		public SystemTrayIconInfoBalloonFlags InfoBalloonFlags;
		public Guid Guid;
		private int infoBalloonIcon;

		public IntPtr WindowHandle => new IntPtr(windowHandle);
		public IntPtr IconHandle => new IntPtr(iconHandle);
		public IntPtr InfoBalloonIcon => new IntPtr(infoBalloonIcon);

		public int Timeout { get => TimeoutOrVersion; }
		public int Version { get => TimeoutOrVersion; }
		public IntPtr Identifier => WindowHandle + (int)Id;

		public void Update(SystemTrayNotificationData newData)
		{
			if (newData.Flags.HasFlag(SystemTrayNotificationFlags.CallbackMessageValid))
				CallbackMessage = newData.CallbackMessage;
			if (newData.Flags.HasFlag(SystemTrayNotificationFlags.IconHandleValid))
				iconHandle = newData.iconHandle;
			if (newData.Flags.HasFlag(SystemTrayNotificationFlags.ToolTipValid))
				ToolTip = newData.ToolTip;
			if (newData.Flags.HasFlag(SystemTrayNotificationFlags.StateValid))
			{
				State = newData.State & newData.StateMask;
				StateMask = newData.StateMask;
			}
		}
	}

	internal class SystemTrayNotificationEventArgs : EventArgs
	{
		public SystemTrayNotificationType Type { get; private set; }
		public SystemTrayNotificationData Data { get; private set; }
		public IntPtr ReturnValue { get; private set; }

		public SystemTrayNotificationEventArgs(SystemTrayNotificationType type, SystemTrayNotificationData data)
		{
			Type = type;
			Data = data;
			ReturnValue = IntPtr.Zero;
		}
	}

	internal delegate void SystemTrayNotificationEventHandler(object sender, SystemTrayNotificationEventArgs e);

	internal class SystemTrayNotificationService : IDisposable
	{
		private SystemTrayNotificationEventHandler systemTrayNotification;

		private IntPtr windowHandle;

		public SystemTrayNotificationService()
		{
			Initialize();
		}

		public void Dispose()
		{
			Uninitialize();
		}

		public void RegisterNotificationEvent(SystemTrayNotificationEventHandler eventHandler)
		{
			systemTrayNotification += eventHandler;
			if (User32.SendMessage(new IntPtr(HWND_BROADCAST), WM_TASKBARCREATED, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero)
			{
				Debugger.Break();
			}
		}

		public void UnregisterNotificationEvent(SystemTrayNotificationEventHandler eventHandler)
		{
			systemTrayNotification -= eventHandler;
		}
		
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		static extern IntPtr CreateWindowEx(int dwExStyle, UInt16 lpClassName, [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
		
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		static extern UInt16 RegisterClass([In] ref WNDCLASS lpWndClass);

		[DllImport("user32.dll")]
		static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern bool SendNotifyMessage(int hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct WNDCLASS
		{
			public int style;
			public IntPtr lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszMenuName;
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpszClassName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct COPYDATASTRUCT
		{
			public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
			public int cbData;       // The count of bytes in the message.
			public IntPtr lpData;    // The address of the message.
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		struct TRAYNOTIFYDATA
		{
			public uint dwSignature;
			public uint dwMessage;
			public SystemTrayNotificationData nid;
		}

		const int HWND_BROADCAST = 0xFFFF;
		int WM_TASKBARCREATED = User32.RegisterWindowMessage("TaskbarCreated");
		delegate IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
		WNDCLASS windowClass;
		WndProc windowProcedure;

		private void Initialize()
		{
			IntPtr hInstance = Marshal.GetHINSTANCE(typeof(SystemTrayNotificationService).Module);
			windowProcedure = new WndProc(WindowProcedure);

			windowClass = new();
			windowClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(windowProcedure);
			windowClass.hInstance = hInstance;
			windowClass.lpszClassName = "Shell_TrayWnd";

			ushort classHandle = RegisterClass(ref windowClass);
			if (classHandle == 0)
			{
				Debugger.Break();
			}

			const int WS_EX_TOOLWINDOW = 0x00000080;
			const int WS_TILEDWINDOW = 0xCF0000;
			const int WS_VISIBLE = 0x10000000;
			const int CW_USEDEFAULT = unchecked((int)0x80000000);
			const int WS_POPUP = CW_USEDEFAULT;

			windowHandle = CreateWindowEx(WS_EX_TOOLWINDOW, classHandle, "", WS_POPUP, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
			if (windowHandle == IntPtr.Zero)
			{
				Debugger.Break();
			}

			User32.ShowWindow(windowHandle, User32.SW_SHOW);
			if (!SendNotifyMessage(HWND_BROADCAST, (uint)WM_TASKBARCREATED, IntPtr.Zero, IntPtr.Zero))// == IntPtr.Zero)
			{
				Debugger.Break();
			}
		}

		private void Uninitialize()
		{
			// Destroy window
		}

		private IntPtr WindowProcedure(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
		{
			switch (uMsg)
			{
				case 0x004A: //WM_COPYDATA
					COPYDATASTRUCT cds = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);
					if (cds.dwData == new IntPtr(0x00000001)) //Shell private constant: TCDM_NOTIFY
					{
						TRAYNOTIFYDATA data = Marshal.PtrToStructure<TRAYNOTIFYDATA>(cds.lpData);
						SystemTrayNotificationData notificationData = data.nid;
						if (notificationData.Size != Marshal.SizeOf(typeof(SystemTrayNotificationData)))
							//Debugger.Break();
							break;
						int message = (int)data.dwMessage;
						SystemTrayNotificationEventArgs e = new((SystemTrayNotificationType)message, notificationData);
						systemTrayNotification?.Invoke(this, e);
						return e.ReturnValue;
					}
					break;
			}

			return DefWindowProc(hWnd, uMsg, wParam, lParam);
		}
	}
}
