using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;

namespace SimpleClassicTheme.Taskbar.Native.SystemTray
{
    internal delegate void SystemTrayNotificationEventHandler(object sender, SystemTrayNotificationEventArgs e);

	internal partial class SystemTrayNotificationService : IDisposable
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
            ShowWindow(oldTaskbar, 0);
            SetWindowPos(windowHandle, new(0), 0, 0, 0, 0, 0x0010 | 0x0002 | 0x0008 | 0x0400 | 0x0001);
			bool postMessageResult = PostMessage(new(HWND_BROADCAST), (uint)WM_TASKBARCREATED, 0, 0);
			Debug.Assert(postMessageResult, "Failed to broadcast WM_TASKBARCREATED. (RegisterNotificationEvent)");
		}

		public void UnregisterNotificationEvent(SystemTrayNotificationEventHandler eventHandler)
		{
			systemTrayNotification -= eventHandler;
		}

		public void RegainTrayPriority()
		{
            BringWindowToTop(windowHandle);
		}

        readonly int WM_TASKBARCREATED = RegisterWindowMessage("TaskbarCreated");
		Integer oldTaskbar;
		delegate Integer WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
		WNDCLASS windowClass;
		WndProc windowProcedure;

		private void Initialize()
		{
			oldTaskbar = FindWindow("Shell_TrayWnd", "");
            ShowWindow(oldTaskbar, 0);

			IntPtr hInstance = Marshal.GetHINSTANCE(typeof(SystemTrayNotificationService).Module);
			windowProcedure = new WndProc(WindowProcedure);
			
			windowClass = new()
			{
				lpfnWndProc = Marshal.GetFunctionPointerForDelegate(windowProcedure),
				hInstance = hInstance,
				lpszClassName = "Shell_TrayWnd",
			};

			ushort classHandle = RegisterClass(ref windowClass);
			Debug.Assert(classHandle != 0, "Failed to register class.");

			windowHandle = CreateWindowEx(WS_EX_TOOLWINDOW | WS_EX_TOPMOST, classHandle, "SCT Shell Window", WS_OVERLAPPEDWINDOW | WS_POPUP, 40, 40, 800, 600, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
			Debug.Assert(windowHandle != IntPtr.Zero, "System tray window handle is null.");

            SetWindowPos(windowHandle, new(-1), 0, 0, 0, 0, 0x0010 | 0x0002 | 0x0008 | 0x0400 | 0x0001);
			bool postMessageResult = PostMessage((IntPtr)HWND_BROADCAST, (uint)WM_TASKBARCREATED, 0, 0);
			Debug.Assert(postMessageResult, "Failed to broadcast WM_TASKBARCREATED. (Initialize)");
		}

		private void Uninitialize()
		{
            // Destroy window
            DestroyWindow(windowHandle);
		}

		private Integer WindowProcedure(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
		{
			switch (uMsg)
			{
				case WM_COPYDATA: //WM_COPYDATA
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
                        SendMessage(oldTaskbar, WM_COPYDATA, wParam, lParam);
					}
					break;
				//case WM_PAINT:
				//	Graphics g = Graphics.FromHwnd(hWnd);
				//	g.Clear(SystemColors.Control);
				//	g.Dispose();
				//	break;
				//case WM_RBUTTONDOWN:
				//	_ = SendMessage(new IntPtr(HWND_BROADCAST), WM_TASKBARCREATED, IntPtr.Zero, IntPtr.Zero);
				//	break;
			}

			return DefWindowProc(hWnd, uMsg, wParam, lParam);
		}
	}
}
