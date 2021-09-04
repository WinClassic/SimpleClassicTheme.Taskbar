using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Common.Native;
using SimpleClassicTheme.Taskbar.Native.SystemTray;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using static SimpleClassicTheme.Taskbar.Native.Headers.WinDef;
using static SimpleClassicTheme.Taskbar.Native.Headers.WinUser;
using static SimpleClassicTheme.Taskbar.Native.Headers.HandleAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.ProcessThreadsAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.MemoryAPI;
using static SimpleClassicTheme.Taskbar.Native.Headers.AppModel;

namespace SimpleClassicTheme.Taskbar.Native
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
		internal static string GetAppUserModelId(int pid)
		{
			IntPtr process = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
			if (process == IntPtr.Zero)
				return "Couldn't open process";

			uint size = 64;
		callGetApplicationUserModelId:
			StringBuilder buffer = new((int)size);
			int result = GetApplicationUserModelId(process, ref size, buffer);
			switch (result)
			{
				case ERROR_SUCCESS:
					break;
				case ERROR_INSUFFICIENT_BUFFER:
					goto callGetApplicationUserModelId;
				case APPMODEL_ERROR_NO_APPLICATION:
					CloseHandle(process);
					return "No user model id";
			}
			CloseHandle(process);
			return buffer.ToString();
		}

		internal static TBBUTTONINFO[] GetTrayButtons(IntPtr sysTray)
		{
			bool Is64Bit = IntPtr.Size == 8;
			int tbButtonSize = Marshal.SizeOf(Is64Bit ? typeof(TBBUTTON64) : typeof(TBBUTTON32));

            _ = GetWindowThreadProcessId(sysTray, out uint trayProcess);
			
			IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, (int)trayProcess);
			if (hProcess == IntPtr.Zero)
			{
				Logger.Instance.Log(LoggerVerbosity.Basic, "CppHelper/GetTrayButtons", "Could not get tray buttons: OpenProcess failed.");
				return Array.Empty<TBBUTTONINFO>();
			}

			IntPtr dataPtr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)tbButtonSize, MEM_COMMIT, PAGE_READWRITE);
			if (dataPtr == IntPtr.Zero)
			{
				// int errocode = Marshal.GetLastWin32Error();
				Logger.Instance.Log(LoggerVerbosity.Basic, "CppHelper/GetTrayButtons", "Could not get tray buttons: VirtualAllocEx failed.");
				return Array.Empty<TBBUTTONINFO>();
			}

			int count = (int)SendMessage(sysTray, TB_BUTTONCOUNT, 0, 0);
			TBBUTTONINFO[] tBBUTTONINFOs = new TBBUTTONINFO[count];
			for (int i = 0; i < count; i++)
			{
				SendMessage(sysTray, TB_GETBUTTON, new(i), dataPtr);

				dynamic tbButton;

                if (Is64Bit)
                {
					tbButton = NativeHelpers.ReadProcessMemoryStructure<TBBUTTON64>(hProcess, dataPtr, tbButtonSize);
                }
                else
                {
					tbButton = NativeHelpers.ReadProcessMemoryStructure<TBBUTTON32>(hProcess, dataPtr, tbButtonSize);
				}

				TRAYDATA trData = NativeHelpers.ReadProcessMemoryStructure<TRAYDATA>(hProcess, tbButton.dwData);

				// GetWindowThreadProcessId(trData.hwnd, out uint iconPid);

				string tooltip = NativeHelpers.ReadNullTerminatedString(hProcess, tbButton.iString);
				bool isVisible = (tbButton.fsState & TBSTATE_HIDDEN) == 0;

				tBBUTTONINFOs[i] = new()
				{
					toolTip = tooltip,
					hwnd = trData.hwnd,
					visible = isVisible,
					icon = trData.hIcon,
					callbackMessage = trData.uCallbackMessage,
					id = trData.uID
				};
			}

			VirtualFreeEx(hProcess, dataPtr, (uint)tbButtonSize, MEM_RELEASE);
			CloseHandle(hProcess);

			return tBBUTTONINFOs;
		}

		internal static int GetTrayButtonCount(IntPtr sysTray)
		{
			return (int)SendMessage(sysTray, TB_BUTTONCOUNT, 0, 0);
		}


		internal static void SetWorkingArea(RECT rect, bool sendChange, IEnumerable<IntPtr> windows)
		{
            SPIF fWinIni = sendChange ? SPIF.SPIF_SENDWININICHANGE | SPIF.SPIF_UPDATEINIFILE : 0;
            
			if (!SystemParametersInfo((uint)SPI.SPI_SETWORKAREA, 0, ref rect, (uint)fWinIni))
			{
				int errorCode = Marshal.GetLastWin32Error();
				Logger.Instance.Log(LoggerVerbosity.Basic, "UnmanagedCode/SetWorkArea", $"Could not set the working area. {errorCode}");
				throw new Win32Exception(errorCode);
			}

			foreach (IntPtr ptr in windows)
			{
                WINDOWPLACEMENT windowPlacement = new()
                {
                    Length = Marshal.SizeOf<WINDOWPLACEMENT>()
                };

                GetWindowPlacement(ptr, ref windowPlacement);

				if (windowPlacement.ShowCmd == SW_SHOWMAXIMIZED)
				{
                    SendMessage(ptr, WM_SYSCOMMAND, SC_RESTORE, 0);
                    SendMessage(ptr, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
				}
			}
		}
	}
}
