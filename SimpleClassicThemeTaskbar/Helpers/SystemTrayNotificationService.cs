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

	internal enum SystemTrayNotificationFlags
	{
		None = 0,
		CallbackMessageValid = 1,
		IconHandleValid = 2,
		ToolTipValid = 4,
		StateValid = 8,

	}

	internal enum SystemTrayIconState
	{
		None = 0,
		Hidden = 1,
		IconHandleShared = 2
	}

	internal enum SystemTrayIconInfoBalloonFlags
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
		private int Size = Marshal.SizeOf(typeof(SystemTrayNotificationData));
		public IntPtr WindowHandle;
		public int Id;
		public SystemTrayNotificationFlags Flags;
		public int CallbackMessage;
		public IntPtr IconHandle;
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
		public IntPtr InfoBalloonIcon;

		public int Timeout { get => TimeoutOrVersion; }
		public int Version { get => TimeoutOrVersion; }
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
		}

		public void UnregisterNotificationEvent(SystemTrayNotificationEventHandler eventHandler)
		{
			systemTrayNotification -= eventHandler;
		}
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr CreateWindow(string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr RegisterClass(ref WNDCLASS lpWndClass);

		[DllImport("user32.dll")]
		static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		static extern bool SendNotifyMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct WNDCLASS
		{
			[MarshalAs(UnmanagedType.U4)]
			public int style;
			public IntPtr lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			public string lpszMenuName;
			public string lpszClassName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct COPYDATASTRUCT
		{
			public IntPtr dwData;    // Any value the sender chooses.  Perhaps its main window handle?
			public int cbData;       // The count of bytes in the message.
			public IntPtr lpData;    // The address of the message.
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		struct TRAYNOTIFYDATA
		{
			public int dwSignature;
			public int dwMessage;
			public SystemTrayNotificationData nid;
		}


		delegate IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

		private void Initialize()
		{
			IntPtr hInstance = Marshal.GetHINSTANCE(typeof(SystemTrayNotificationService).Module);
			
			WNDCLASS windowClass = new();
			windowClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(new WndProc(WindowProcedure));
			windowClass.hInstance = hInstance;
			windowClass.lpszClassName = "Shell_TrayWnd";

			if (RegisterClass(ref windowClass) == IntPtr.Zero)
			{
				Debugger.Break();
			}

			const int WS_TILEDWINDOW = 0xCF0000;
			const int WS_VISIBLE = 0x10000000;
			const int CW_USEDEFAULT = unchecked((int)0x80000000);
			const int HWND_BROADCAST = 0xFFFF;

		    windowHandle = CreateWindow("Shell_TrayWnd", "SCTT_TRAY", WS_TILEDWINDOW | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
			if (windowHandle == IntPtr.Zero)
			{
				Debugger.Break();
			}

			User32.ShowWindow(windowHandle, User32.SW_SHOW);
			SendNotifyMessage(new IntPtr(HWND_BROADCAST), (uint)User32.RegisterWindowMessage("TaskbarCreated"), IntPtr.Zero, IntPtr.Zero);
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
					if (lParam == new IntPtr(0x00000001)) //Shell private constant: TCDM_NOTIFY
					{
						COPYDATASTRUCT cds = Marshal.PtrToStructure<COPYDATASTRUCT>(wParam);
						if (cds.cbData != Marshal.SizeOf(typeof(TRAYNOTIFYDATA)))
							break;
						TRAYNOTIFYDATA data = Marshal.PtrToStructure<TRAYNOTIFYDATA>(cds.lpData);
						SystemTrayNotificationData notificationData = data.nid;
						int message = data.dwMessage;
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
