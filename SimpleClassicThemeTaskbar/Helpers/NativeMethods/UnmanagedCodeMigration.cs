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

		#region NativeConstants
		const int TB_BUTTONCOUNT = 0x0418;
		#endregion NativeConstants

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
		struct TBUTTON32
		{
			int iBitmap;
			int idCommand;
			byte fsState;
			byte fsStyle;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			byte[] bReserved;
			UIntPtr dwData;
			IntPtr iString;
		}

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		struct TBUTTON64
		{
			int iBitmap;
			int idCommand;
			byte fsState;
			byte fsStyle;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			byte[] bReserved;
			UIntPtr dwData;
			IntPtr iString;
		}

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
			int VirtualDesktopCreated(IntPtr pDesktop);
			int VirtualDesktopDestroyBegin(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			int VirtualDesktopDestroyFailed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			int VirtualDesktopDestroyed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback);
			int ViewVirtualDesktopChanged(IntPtr pView);
			int CurrentVirtualDesktopChanged(IntPtr pDesktopOld, IntPtr pDesktopNew);
		}

		[ComImport]
		[Guid("0cd45e71-d927-4f15-8b0a-8fef525337bf")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IVirtualDesktopNotificationService
		{
			int Register(ref IVirtualDesktopNotification pNotification, out IntPtr pdwCookie);
			int Unregister(IntPtr dwCookie);
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

			public int VirtualDesktopCreated(IntPtr pDesktop) => 0;
			public int VirtualDesktopDestroyBegin(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) => 0;
			public int VirtualDesktopDestroyFailed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) => 0;
			public int VirtualDesktopDestroyed(IntPtr pDesktopDestroyed, IntPtr pDesktopFallback) => 0;
			public int ViewVirtualDesktopChanged(IntPtr pView) => 0;
			public int CurrentVirtualDesktopChanged(IntPtr pDesktopOld, IntPtr pDesktopNew)
			{
				CurrentDesktopChanged?.Invoke(this, EventArgs.Empty);
				return 0;
			}
		}
		#endregion

	    static IVirtualDesktopNotificationService virtualDesktopNotificationService;
		static IVirtualDesktopManager virtualDesktopManager;

		internal static int GetTrayButtonCount(IntPtr sysTray)
		{
			return (int)User32.SendMessage(sysTray, TB_BUTTONCOUNT, 0, 0);
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
		
		internal static IntPtr RegisterVdmNotification(IVirtualDesktopNotification notification)
		{
			//virtualDesktopNotificationService.Register(ref notification, out IntPtr cookie);
			//return cookie;
			return IntPtr.Zero;
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
