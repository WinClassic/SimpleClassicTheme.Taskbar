using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
	/// <summary>
	/// This class is intended for the functions currently present in SimpleClassicThemeTaskbar.UnmanagedCode
	/// We'll migrate everything here first, and then put it in the appropriate areas of SCTT accordingly.
	/// Don't move or rename anything out of here before UnmanagedCode is fully migrated.
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
		struct POINT
		{
			int X;
			int Y;
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

		static Type vdmType;
		static object vdmObject;

		internal static void InitializeVdmInterface()
		{
		    vdmType = Type.GetTypeFromCLSID(new Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b"));
			vdmObject = Activator.CreateInstance(vdmType);
		}

		internal static void UnitializeVdmInterface()
		{
			vdmType.InvokeMember("Release", System.Reflection.BindingFlags.InvokeMethod, null, vdmObject, Array.Empty<object>());
		}

		internal static bool IsWindowOnCurrentVirtualDesktop(IntPtr window)
		{
			if (vdmObject is not null)
			{
				bool isWndOnDekstop = false;
				ref bool pointer = ref isWndOnDekstop;
				vdmType.InvokeMember("IsWindowOnCurrentVirtualDesktop", System.Reflection.BindingFlags.InvokeMethod, null, vdmObject, new object[] { window, pointer });
				return isWndOnDekstop;
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
