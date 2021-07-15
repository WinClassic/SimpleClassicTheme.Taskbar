using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
	/// <summary>
	/// This class is intended for the functions currently present in SimpleClassicThemeTaskbar.UnmanagedCode
	/// We'll migrate everything here first, and then put it in the appropriate areas of SCTT accordingly.
	/// Don't move or rename anything out of here before UnmanagedCode is fully migrated.
	/// 
	/// Update: Migration is done. Class is ready to be taken apart
	/// </summary>
	internal static class UnmanagedCodeMigration
	{
		#region NativeImports
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref RECT pvParam, SPIF fWinIni);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
		#endregion

		#region NativeEnums
		enum SPI : uint
		{
			SPI_SETWORKAREA = 0x002F
		}

		enum SPIF : uint
		{
			None = 0x00,
			SPIF_UPDATEINIFILE = 0x01,
			SPIF_SENDCHANGE = 0x02,
			SPIF_SENDWININICHANGE = 0x02
		}
		#endregion

		#region NativeStructures
		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		struct TBBUTTON32
		{
			public int iBitmap;
			public int idCommand;
			public byte fsState;
			public byte fsStyle;
		    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public byte[] bReserved;
			public IntPtr dwData;
			public IntPtr iString;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		struct TBBUTTON64
		{
			public int iBitmap;
			public int idCommand;
			public byte fsState;
			public byte fsStyle;
		    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public byte[] bReserved;
			public IntPtr dwData;
			public IntPtr iString;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		struct TRAYDATA
		{
			public IntPtr hwnd;
			public uint uID;
			public uint uCallbackMessage;
		    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public byte[] Reserved;
			public IntPtr hIcon;
		}

		internal struct TBBUTTONINFO
		{
			public IntPtr hwnd;
			public ulong pid;
			public string toolTip;
			public bool visible;
			public IntPtr icon;
			public uint callbackMessage;
			public uint id;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		struct POINT
		{
			public int X;
			public int Y;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
	    struct WINDOWPLACEMENT
		{
			public int Length;
			public int Flags;
			public int ShowCmd;
			public POINT MinPosition;
			public POINT MaxPosition;
			public RECT NormalPosition;
		}
		#endregion

		#region NativeInterfaces
		[ComImport]
		[Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IVirtualDesktopManager
		{
			bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);
			Guid GetWindowDesktopId(IntPtr topLevelWindow);
			void MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
		}

		[ComImport]
		[Guid("c179334c-4295-40d3-bea1-c654d965605a")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IVirtualDesktopNotification
		{
			void VirtualDesktopCreated(IntPtr pDesktop);
			void VirtualDesktopDestroyBegin(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			void VirtualDesktopDestroyFailed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			void VirtualDesktopDestroyed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			void ViewVirtualDesktopChanged(IntPtr pView);
			void CurrentVirtualDesktopChanged(IntPtr pDesktopOld, IntPtr pDesktopNew);
		}

		[ComImport]
		[Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IVirtualDesktopNotificationService
		{
			int Register(IVirtualDesktopNotification pNotification, out uint dwCookie);
			int Unregister(uint dwCookie);
		}

		[ComImport]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IServiceProvider
		{
			[PreserveSig]
			[return: MarshalAs(UnmanagedType.I4)]
			int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		}

		public class VirtualDesktopNotification : IVirtualDesktopNotification
		{
			public EventHandler CurrentDesktopChanged;

			public void VirtualDesktopCreated(IntPtr pDesktop) { }
			public void VirtualDesktopDestroyBegin(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) { }
			public void VirtualDesktopDestroyFailed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) { }
			public void VirtualDesktopDestroyed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) { }
			public void ViewVirtualDesktopChanged(IntPtr pView) { }
			public void CurrentVirtualDesktopChanged(IntPtr pDesktopOld, IntPtr pDesktopNew)
			{
				CurrentDesktopChanged?.Invoke(this, EventArgs.Empty);
			}
		}
		#endregion

	    static IVirtualDesktopNotificationService virtualDesktopNotificationService;
		static IVirtualDesktopManager virtualDesktopManager;

		internal static string GetAppUserModelId(int pid)
		{
			IntPtr process = Kernel32.OpenProcess(Kernel32.PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
			if (process == IntPtr.Zero)
				return "Couldn't open process";

			uint size = 64;
		callGetApplicationUserModelId:
			StringBuilder buffer = new((int)size);
			int result = Kernel32.GetApplicationUserModelId(process, ref size, buffer);
			switch (result)
			{
				case Kernel32.ERROR_SUCCESS:
					break;
				case Kernel32.ERROR_INSUFFICIENT_BUFFER:
					goto callGetApplicationUserModelId;
				case Kernel32.APPMODEL_ERROR_NO_APPLICATION:
					Kernel32.CloseHandle(process);
					return "No user model id";
			}
			Kernel32.CloseHandle(process);
			return buffer.ToString();
		}

		internal static TBBUTTONINFO[] GetTrayButtons(IntPtr sysTray)
		{
			bool Is64Bit = IntPtr.Size == 8;
			int tbButtonSize = Marshal.SizeOf(Is64Bit ? new TBBUTTON64().GetType() : new TBBUTTON32().GetType());

			User32.GetWindowThreadProcessId(sysTray, out uint trayProcess);
			IntPtr hProcess = Kernel32.OpenProcess(Kernel32.PROCESS_ALL_ACCESS, false, (int)trayProcess);
			if (hProcess == IntPtr.Zero)
			{
				Logger.Log(LoggerVerbosity.Basic, "CppHelper/GetTrayButtons", "Could not get tray buttons: OpenProcess failed.");
				return Array.Empty<TBBUTTONINFO>();
			}
			IntPtr dataPtr = Kernel32.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)tbButtonSize, Kernel32.MEM_COMMIT, Kernel32.PAGE_READWRITE);
			if (dataPtr == IntPtr.Zero)
			{
				int errocode = Marshal.GetLastWin32Error();
				Logger.Log(LoggerVerbosity.Basic, "CppHelper/GetTrayButtons", "Could not get tray buttons: VirtualAllocEx failed.");
				return Array.Empty<TBBUTTONINFO>();
			}
			int count = (int)User32.SendMessage(sysTray, User32.TB_BUTTONCOUNT, 0, 0);
			TBBUTTONINFO[] tBBUTTONINFOs = new TBBUTTONINFO[count];
			if (Is64Bit)
			{
				for (int i = 0; i < count; i++)
				{
					TBBUTTON64 tbButton = new();
					TRAYDATA trData = new();

					User32.SendMessage(sysTray, User32.TB_GETBUTTON, i, (int)dataPtr);

					byte[] tbButtonBytes = new byte[tbButtonSize];
					Kernel32.ReadProcessMemory(hProcess, dataPtr, tbButtonBytes, tbButtonSize, out var bytesRead);
					GCHandle tbButtonHandle = GCHandle.Alloc(tbButtonBytes, GCHandleType.Pinned);
					tbButton = (TBBUTTON64) Marshal.PtrToStructure(tbButtonHandle.AddrOfPinnedObject(), tbButton.GetType());
					tbButtonHandle.Free();

					byte[] trDataBytes = new byte[Marshal.SizeOf<TRAYDATA>()];
					Kernel32.ReadProcessMemory(hProcess, tbButton.dwData, trDataBytes, trDataBytes.Length, out bytesRead);
					GCHandle trDataHandle = GCHandle.Alloc(trDataBytes, GCHandleType.Pinned);
					trData = (TRAYDATA) Marshal.PtrToStructure(trDataHandle.AddrOfPinnedObject(), trData.GetType());
					trDataHandle.Free();

					User32.GetWindowThreadProcessId(trData.hwnd, out uint iconPid);

					byte[] buffer = new byte[2];
					StringBuilder toolTip = new(1024);
					if ((tbButton.fsState & User32.TBSTATE_HIDDEN) == 0)
					{
						IntPtr pTip = tbButton.iString;
						while (true)
						{
							Kernel32.ReadProcessMemory(hProcess, pTip, buffer, 2, out bytesRead);
							string character = Encoding.Unicode.GetString(buffer);
							if (character == "\0")
								break;
							toolTip.Append(character);
							pTip += 2;
						}
					}

					TBBUTTONINFO buttonInfo = new();
					buttonInfo.toolTip = toolTip.ToString();
					buttonInfo.hwnd = trData.hwnd;
					buttonInfo.visible = (tbButton.fsState & User32.TBSTATE_HIDDEN) == 0;
					buttonInfo.icon = trData.hIcon;
					buttonInfo.callbackMessage = trData.uCallbackMessage;
					buttonInfo.id = trData.uID;
					tBBUTTONINFOs[i] = buttonInfo;
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					TBBUTTON32 tbButton = new();
					TRAYDATA trData = new();

					User32.SendMessage(sysTray, User32.TB_GETBUTTON, i, (int)dataPtr);

					byte[] tbButtonBytes = new byte[tbButtonSize];
					Kernel32.ReadProcessMemory(hProcess, dataPtr, tbButtonBytes, tbButtonSize, out var bytesRead);
					GCHandle tbButtonHandle = GCHandle.Alloc(tbButtonBytes, GCHandleType.Pinned);
					tbButton = (TBBUTTON32)Marshal.PtrToStructure(tbButtonHandle.AddrOfPinnedObject(), tbButton.GetType());
					tbButtonHandle.Free();

					byte[] trDataBytes = new byte[Marshal.SizeOf<TRAYDATA>()];
					Kernel32.ReadProcessMemory(hProcess, tbButton.dwData, trDataBytes, trDataBytes.Length, out bytesRead);
					GCHandle trDataHandle = GCHandle.Alloc(trDataBytes, GCHandleType.Pinned);
					trData = (TRAYDATA)Marshal.PtrToStructure(trDataHandle.AddrOfPinnedObject(), trData.GetType());
					trDataHandle.Free();

					User32.GetWindowThreadProcessId(trData.hwnd, out uint iconPid);

					byte[] buffer = new byte[1];
					StringBuilder toolTip = new(1024);
					if ((tbButton.fsState & User32.TBSTATE_HIDDEN) == 0)
					{
						IntPtr pTip = tbButton.iString;
						while (true)
						{
							Kernel32.ReadProcessMemory(hProcess, pTip, buffer, 2, out bytesRead);
							string character = Encoding.Unicode.GetString(buffer);
							if (character == "\0")
								break;
							toolTip.Append(character);
							pTip += 2;
						}
					}

					TBBUTTONINFO buttonInfo = new();
					buttonInfo.toolTip = toolTip.ToString();
					buttonInfo.hwnd = trData.hwnd;
					buttonInfo.visible = (tbButton.fsState & User32.TBSTATE_HIDDEN) != 0;
					buttonInfo.icon = trData.hIcon;
					buttonInfo.callbackMessage = trData.uCallbackMessage;
					buttonInfo.id = trData.uID;
					tBBUTTONINFOs[i] = buttonInfo;
				}
			}

			Kernel32.VirtualFreeEx(hProcess, dataPtr, (uint)tbButtonSize, Kernel32.MEM_RELEASE);
			Kernel32.CloseHandle(hProcess);

			return tBBUTTONINFOs;
		}

		internal static int GetTrayButtonCount(IntPtr sysTray)
		{
			return (int)User32.SendMessage(sysTray, User32.TB_BUTTONCOUNT, 0, 0);
		}

		internal static void InitializeVdmInterface()
		{
		    Type vdmType = Type.GetTypeFromCLSID(new Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a"));
			virtualDesktopManager = (IVirtualDesktopManager) Activator.CreateInstance(vdmType);

			Type spType = Type.GetTypeFromCLSID(new Guid("c2f03a33-21f5-47fa-b4bb-156362a2f239"));
			IServiceProvider serviceProvider = (IServiceProvider) Activator.CreateInstance(spType);
			serviceProvider.QueryService(new Guid("a501fdec-4a09-464c-ae4e-1b9c21b84918"), typeof(IVirtualDesktopNotificationService).GUID, out object ppvObject);
			virtualDesktopNotificationService = (IVirtualDesktopNotificationService)ppvObject;
		}
		
		internal static uint RegisterVdmNotification(IVirtualDesktopNotification notification)
		{
			return 0U;
			virtualDesktopNotificationService.Register(notification, out uint cookie);
			return cookie;
		}

		internal static void UnregisterVdmNotification(uint notificationCookie)
		{
			virtualDesktopNotificationService.Unregister(notificationCookie);
		}

		internal static bool IsWindowOnCurrentVirtualDesktop(IntPtr window)
		{
			if (virtualDesktopManager is not null)
			{
				return virtualDesktopManager.IsWindowOnCurrentVirtualDesktop(window);
				//bool isWndOnDekstop = false;
				//ref bool pointer = ref isWndOnDekstop;
				//vdmType.InvokeMember("IsWindowOnCurrentVirtualDesktop", System.Reflection.BindingFlags.InvokeMethod, null, vdmObject, new object[] { window, pointer });
				//return isWndOnDekstop;
			}
			return true;
		}

		internal static void SetWorkingArea(RECT rect, bool sendChange, IEnumerable<IntPtr> windows)
		{
			const int SC_MAXIMIZE = 0xF030;
			const int SC_RESTORE = 0xF120;
			const int SW_SHOWMAXIMIZED = 3;
			const int WM_SYSCOMMAND = 0x0112;

			if (!SystemParametersInfo(SPI.SPI_SETWORKAREA, 0, ref rect, sendChange ? SPIF.SPIF_SENDWININICHANGE | SPIF.SPIF_UPDATEINIFILE : 0))
			{
				int errorCode = Marshal.GetLastWin32Error();
				Logger.Log(LoggerVerbosity.Basic, "UnmanagedCode/SetWorkArea", $"Could not set the working area. {errorCode}");
				throw new Win32Exception(errorCode);
			}

			foreach (IntPtr ptr in windows)
			{
				WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
				windowPlacement.Length = Marshal.SizeOf<WINDOWPLACEMENT>();
				GetWindowPlacement(ptr, ref windowPlacement);
				if (windowPlacement.ShowCmd == SW_SHOWMAXIMIZED)
				{
					User32.SendMessage(ptr, WM_SYSCOMMAND, SC_RESTORE, 0);
					User32.SendMessage(ptr, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
				}
			}
		}
	}
}
