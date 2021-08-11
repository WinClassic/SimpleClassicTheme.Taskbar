using SimpleClassicTheme.Common.Logging;
using SimpleClassicTheme.Taskbar.Native.Interfaces;

using System;

using IServiceProvider = SimpleClassicTheme.Taskbar.Native.Interfaces.IServiceProvider;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public static class VirtualDesktops
    {
        private static IVirtualDesktopNotificationService s_notificationService;
		private static IVirtualDesktopManager s_manager;
        public static bool IsInitialized { get; private set; }
		
		public static void InitializeVdmInterface()
		{
			Type vdmType = Type.GetTypeFromCLSID(new Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a"));
			s_manager = (IVirtualDesktopManager)Activator.CreateInstance(vdmType);

			Type spType = Type.GetTypeFromCLSID(new Guid("c2f03a33-21f5-47fa-b4bb-156362a2f239"));
			IServiceProvider serviceProvider = (IServiceProvider)Activator.CreateInstance(spType);
			serviceProvider.QueryService(new Guid("a501fdec-4a09-464c-ae4e-1b9c21b84918"), typeof(IVirtualDesktopNotificationService).GUID, out object ppvObject);
			s_notificationService = (IVirtualDesktopNotificationService)ppvObject;

			IsInitialized = s_manager is not null;
		}

		public static uint RegisterVdmNotification(IVirtualDesktopNotification notification)
		{
			try
			{
				s_notificationService.Register(notification, out uint cookie);
				return cookie;
			}
			catch (Exception e)
			{
				Logger.Instance.Log(LoggerVerbosity.Basic, "VDMService", "Failed to register VDM Notification. Reason: " + e.Message);
				return 0U;
			}
		}

		public static void UnregisterVdmNotification(uint notificationCookie)
		{
			if (IsInitialized)
			{
				s_notificationService?.Unregister(notificationCookie);
			}
		}

		public static bool IsWindowOnCurrentVirtualDesktop(IntPtr window)
		{
			if (IsInitialized)
			{
				return s_manager.IsWindowOnCurrentVirtualDesktop(window);
				//bool isWndOnDekstop = false;
				//ref bool pointer = ref isWndOnDekstop;
				//vdmType.InvokeMember("IsWindowOnCurrentVirtualDesktop", System.Reflection.BindingFlags.InvokeMethod, null, vdmObject, new object[] { window, pointer });
				//return isWndOnDekstop;
			}
			return true;
		}
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
}
