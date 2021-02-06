#include "ApplicationWindow.h"
#include <wingdi.h>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		ApplicationWindowInfo::ApplicationWindowInfo(HWND application)
		{
			WindowHandle = application;
			GetWindowThreadProcessId(WindowHandle, &PID);
		}

		TCHAR* ApplicationWindowInfo::GetWindowClassName()
		{
			TCHAR* buffer = (TCHAR*) malloc(sizeof(TCHAR) * 100);
			GetClassName(WindowHandle, buffer, 100);
			return buffer;
		}

		TCHAR* ApplicationWindowInfo::GetTitle()
		{
			TCHAR* buffer = (TCHAR*)malloc(sizeof(TCHAR) * 100);
			GetWindowText(WindowHandle, buffer, 100);
			return buffer;
		}

		WINDOWINFO ApplicationWindowInfo::GetWindowInfo()
		{
			WINDOWINFO wi = { 0 };
			wi.cbSize = sizeof(WINDOWINFO);
			::GetWindowInfo(WindowHandle, &wi);
			return wi;
		}

		HICON ApplicationWindowInfo::GetIcon()
		{
			HICON icon = 0;

			icon = (HICON)SendMessage(WindowHandle, WM_GETICON, ICON_SMALL2, 0);
			if (icon == NULL)
				icon = (HICON)SendMessage(WindowHandle, WM_GETICON, ICON_SMALL, 0);
			if (icon == NULL)
				icon = (HICON)GetClassLongPtr(WindowHandle, GCLP_HICONSM);
			if (icon == NULL)
				icon = (HICON)SendMessage(WindowHandle, WM_GETICON, ICON_BIG, 0);
			if (icon == NULL)
				icon = (HICON)GetClassLongPtr(WindowHandle, GCLP_HICON);

			return icon;
		}

		ApplicationWindow::ApplicationWindow(HWND application) : Info(application), ButtonWindow()
		{
			IsGrouped = false;
			IsPartOfGroup = false;
			children = std::vector<ApplicationWindow>();
			//extraWindowStyles = WS_VISIBLE | WS_CHILD | BS_PUSHBUTTON | BS_LEFT /*BS_OWNERDRAW*/;
			createParams.style = WS_VISIBLE | WS_CHILD | /*BS_PUSHBUTTON | BS_LEFT*/ BS_OWNERDRAW;
			createParams.hMenu = (HMENU)IDC_APPLICATION_WINDOW;
		}

		ApplicationWindow::~ApplicationWindow() 
		{
			//DestroyWindow(Info.WindowHandle);
		}

		void ApplicationWindow::ClickHandler()
		{
			// TODO implement click handler better
			BringWindowToTop(Info.WindowHandle);
		}

		void ApplicationWindow::AddChildWindow(ApplicationWindow window)
		{
			// NIY: grouping
		}

		void ApplicationWindow::RemoveChildWindow(ApplicationWindow window)
		{
			// NIY: grouping
		}

		bool ApplicationWindow::HandleMatches(HWND handle)
		{
			// TODO: implement handle matching for groups
			return handle == Info.WindowHandle;
		}
	}
}