#include "Taskbar.h"
#include <VersionHelpers.h>
#include <dwmapi.h>
#pragma comment (lib, "dwmapi.lib")

#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")

#define WM_UPDATETASKBAR		WM_USER
#define WM_REPOSITIONTASKBAR	WM_USER + 1
#define DEBUG_DRAWBG false
#define DEBUG_USETAB true

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		static bool isDone = false;
		void Taskbar::DoAlternativeUpdate()
		{
			if (isDone)
				return;
			isDone = true;

			// Give toolbar new handles
			taskListWindow->Update(applicationHandleList);
		}

		bool Taskbar::windowClassRegistered = false;
		bool Taskbar::secondaryWindowClassRegistered = false;

		UnmanagedCode Taskbar::cppCode = UnmanagedCode();
		HFONT Taskbar::font = NULL;
		HFONT Taskbar::boldFont = NULL;

		std::vector<HWND>* Taskbar::windowsTaskbars = new std::vector<HWND>();
		std::vector<HWND>* Taskbar::secondaryTaskbars = new std::vector<HWND>();
		std::vector<HWND>* Taskbar::applicationHandleList = new std::vector<HWND>();
		
		//Constructor
		Taskbar::Taskbar(bool isPrimary) : Window()
		{
			IsPrimary = isPrimary;
			createParams.lpszClass = IsPrimary ? L"SCTT_Shell_TrayWnd" : L"SCTT_Shell_SecondaryTrayWnd";
			createParams.style = WS_CLIPCHILDREN;
			createParams.dwExStyle = WS_EX_COMPOSITED;
			applicationWindowList = std::vector<ApplicationWindow*>();
			applicationWindowHandleMap = std::unordered_map<HWND, ApplicationWindow*>();
			
			if (IsPrimary && !windowClassRegistered)
			{
				RegisterWindowClass();
				windowClassRegistered = true;
			}
			else if (!IsPrimary && !secondaryWindowClassRegistered)
			{
				RegisterWindowClass();
				secondaryWindowClassRegistered = true;
			}

			startWindow = new StartWindow();
			taskListWindow = new TaskListWindow();
			
			CreateWindowHandle();

			if (IsPrimary)
				SetTimer(WindowHandle, 0, 200, NULL);
		}

		//Destructor
		Taskbar::~Taskbar() { }

		//Show the window on a specific screen (Indicated by work area)
		void Taskbar::ShowOnScreen(RECT workArea)
		{
			//ShowWindow(WindowHandle, SW_SHOW);
			SetWindowLong(WindowHandle, GWL_STYLE, 0);
			SetWindowPos(WindowHandle, HWND_TOPMOST, workArea.left, workArea.bottom - 28, workArea.right - workArea.left, 28, SWP_SHOWWINDOW);
			SendMessage(WindowHandle, WM_REPOSITIONTASKBAR, NULL, NULL);
		}

		//Check if the specified HWND is a window to be displayed
		bool Taskbar::IsAltTabWindow(HWND hwnd, TCHAR* className)
		{
			// Check if the OS is Windows 10
			if (IsWindows10OrGreater())
				// Check if the window is on the current Desktop
				if (!cppCode.WindowIsOnCurrentDesktop(hwnd))
					return false;

			HWND root = GetAncestor(hwnd, 3);

			// If the last active popup of the root owner is NOT this window: don't show it
			// This method is described by Raymond Chen in this blogpost:
			// https://devblogs.microsoft.com/oldnewthing/20071008-00/?p=24863
			// Start at the root owner
			HWND hwndWalk = GetAncestor(hwnd, GA_ROOTOWNER);
			// See if we are the last active visible popup
			HWND hwndTry;
			while ((hwndTry = GetLastActivePopup(hwndWalk)) != hwndTry) {
				if (IsWindowVisible(hwndTry)) break;
				hwndWalk = hwndTry;
			}
			if (hwndWalk != hwnd)
				return false;

			// Get WINDOWINFO
			WINDOWINFO wi = { 0 };
			wi.cbSize = sizeof(WINDOWINFO);
			GetWindowInfo(hwnd, &wi);

			// If it's a tool window: don't show it
			if ((wi.dwExStyle & WS_EX_TOOLWINDOW) > 0)
				return false;

			TCHAR* buffer = (TCHAR*)malloc(sizeof(TCHAR) * 100);
			GetWindowText(hwnd, buffer, 100);

			// If it's any of these odd cases: don't show it
			if (lstrcmp(className, L"WorkerW") == 0 ||
				lstrcmp(className, L"Progman") == 0 ||
				lstrcmp(className, L"ThumbnailDeviceHelperWnd") == 0 ||
				lstrcmp(className, L"Windows.UI.Core.CoreWindow") == 0 ||
				lstrcmp(className, L"DV2ControlHost") == 0 ||
				(lstrcmp(className, L"Button") == 0) && lstrcmp(buffer, L"Start") == 0 ||
				lstrcmp(className, L"MsgrIMEWindowClass") == 0 ||
				lstrcmp(className, L"SysShadow") == 0 ||
				wcsstr(className, L"WMP9MediaBarFlyout") != NULL ||
				wcslen(buffer) == 0)
				return false;

			// UWP app
			if (lstrcmp(className, L"ApplicationFrameWindow") == 0)
			{
				// Do an API call to see if app isn't cloaked
				int d;
				DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, &d, sizeof(int));

				// If returned value is not 0, the window is cloaked
				if (d > 0)
				{
					return false;
				}
			}

			// If none of those things failed: Yay, we have a window we should display!
			return true;
		}

		//Window enumeration procedure for retrieving all windows to be displayed and all secondary taskbars
		BOOL CALLBACK Taskbar::enumWindowsProcedure(__in HWND hwnd, __in LPARAM lParam)
		{
			// If window isn't visible it can't possibly be on the taskbar
			if (!IsWindowVisible(hwnd))
				return TRUE;

			TCHAR* buffer = (TCHAR*)malloc(sizeof(TCHAR) * 100);
			if (buffer)
			{
				GetClassName(hwnd, buffer, 100);
				if (lstrcmp(buffer, L"Shell_TrayWnd") == 0 || lstrcmp(buffer, L"Shell_SecondaryTrayWnd") == 0)
				{
					windowsTaskbars->push_back(hwnd);
				}
				else if (lstrcmp(buffer, L"SCTT_Shell_TrayWnd") == 0 || lstrcmp(buffer, L"SCTT_Shell_SecondaryTrayWnd") == 0)
				{
					if ((HWND)lParam != hwnd)
					{
						secondaryTaskbars->push_back(hwnd);
					}
				}
				else
				{
					if (IsAltTabWindow(hwnd, buffer))
					{
						applicationHandleList->push_back(hwnd);
					}
				}
			}

			return TRUE;
		}

		//Window procedure for individual taskbars
		LRESULT CALLBACK Taskbar::windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
		{
			NONCLIENTMETRICS ncm;
			PAINTSTRUCT ps;
			HDC hdc;
			RECT rc;
			
			HWND foregroundWindow;
			MONITORINFO monitorInfo;
			std::vector<ApplicationWindow*> newApplicationWindowList;
			int startingPositionX;
			int applicationWindowWidth;

			LPDRAWITEMSTRUCT dis;
			HICON icon;
			HMODULE moduleHandle;

			switch (msg)
			{
			case WM_CREATE:
				WindowHandle = hwnd;

				// Initialize components
				startWindow->SetParent(WindowHandle);
				taskListWindow->SetParent(WindowHandle);
				//startWindow->SetText(L"Start");
			case WM_REPOSITIONTASKBAR:
			case WM_SYSCOLORCHANGE:
			case WM_FONTCHANGE:
				// Font changed, we need to redraw everything with a new font
				
				// Get nonclient metrics
				ncm = { 0 };
				ncm.cbSize = sizeof(NONCLIENTMETRICS);
				SystemParametersInfo(SPI_GETNONCLIENTMETRICS, ncm.cbSize, &ncm, 0);

				// Disable anti aliasing for created font
				ncm.lfCaptionFont.lfQuality = NONANTIALIASED_QUALITY;

				// Create a bold font from the received information
				boldFont = CreateFontIndirect(&ncm.lfCaptionFont);

				// Set the font weight to FW_REGULAR
				ncm.lfCaptionFont.lfWeight = FW_REGULAR;

				// Create a regular font from the received information
				font = CreateFontIndirect(&ncm.lfCaptionFont);

				// Resize components
				moduleHandle = GetModuleHandle(L"SimpleClassicThemeTaskbar.CodeBridge.dll");
				icon = (HICON)LoadImage(moduleHandle, MAKEINTRESOURCE(IDI_CLASSICWINLOGO), IMAGE_ICON, 16, 16, NULL);
				if (icon != NULL)
				{
					HICON oldIcon = startWindow->GetIcon(false);
					startWindow->SetIcon(icon);
					DestroyIcon(oldIcon);
				}
				else 
				{
					DebugBreak();
				}
				startWindow->SetFont(boldFont);
				SIZE startWindowSize;
				Button_GetIdealSize(startWindow->WindowHandle, &startWindowSize);
				startWindowSize.cx += 4;
				startWindow->SetSize(startWindowSize);
				startWindow->SetPosition(POINT{ 2, 4 });

				taskListWindow->SetFont(font);
				taskListWindow->SetSize(GetSize().cx - 2 - startWindowSize.cx - 4, startWindowSize.cy );
				taskListWindow->SetPosition(POINT{ 2 + startWindowSize.cx + 4, 4 });

				// Resize taskbar and TODO work area
				monitorInfo = { 0 };
				monitorInfo.cbSize = sizeof(MONITORINFO);
				GetMonitorInfo(MonitorFromWindow(WindowHandle, MONITOR_DEFAULTTOPRIMARY), &monitorInfo);
				SetSize(SIZE{ GetSize().cx, startWindowSize.cy + 6 });
				SetPosition(POINT{ monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - (startWindowSize.cy + 6) });

				InvalidateRect(hwnd, NULL, TRUE);
				break;
			case WM_GETFONT:
				return (LRESULT)font;
			case WM_TIMER:
				if (!IsPrimary)
					break;
				// Clear old arrays
				windowsTaskbars->clear();
				secondaryTaskbars->clear();
				applicationHandleList->clear();

				// Filter and sort all top-level windows
				EnumWindows(enumWindowsProcedure, (LPARAM)WindowHandle);

				// Hide all explorer's taskbars
				for (int i = 0; i < windowsTaskbars->size(); i++)
					ShowWindow((*windowsTaskbars)[i], SW_HIDE);

				// Inform other taskbars about update
				for (int i = 0; i < secondaryTaskbars->size(); i++)
					SendMessage((*secondaryTaskbars)[i], WM_UPDATETASKBAR, 0, 0);
			case WM_UPDATETASKBAR:
				
				if (DEBUG_USETAB)
				{
					DoAlternativeUpdate();
					break;
				}

				// Get the foreground window
				foregroundWindow = GetForegroundWindow();

				// If the foreground window is a fullscreen window we hide
				monitorInfo = { 0 };
				monitorInfo.cbSize = sizeof(MONITORINFO);
				GetMonitorInfo(MonitorFromWindow(WindowHandle, MONITOR_DEFAULTTOPRIMARY), &monitorInfo);
				RECT windowRect;
				GetWindowRect(foregroundWindow, &windowRect);
				if (windowRect.left == monitorInfo.rcMonitor.left &&
					windowRect.top == monitorInfo.rcMonitor.top &&
					windowRect.right == monitorInfo.rcMonitor.right &&
					windowRect.bottom == monitorInfo.rcMonitor.bottom)
				{
					// Hide and return
					ShowWindow(WindowHandle, SW_HIDE);
					return TRUE;
				}
				// If hidden, show again
				else if (!IsWindowVisible(WindowHandle))
				{
					ShowWindow(WindowHandle, SW_SHOW);
				}

				// Check if any new window exists, if so: add it
				for (int i = 0; i < applicationHandleList->size(); i++)
				{
					HWND hwnd = (*applicationHandleList)[i];

					bool exists = false;
					for (int j = 0; j < applicationWindowList.size(); j++)
						if (applicationWindowList[j]->HandleMatches(hwnd))
							exists = true;

					if (!exists)
					{
						// NIY: Check if blacklisted first
						applicationWindowList.push_back(new ApplicationWindow(hwnd));
					}
				}
				
				// Make a new list
				newApplicationWindowList = std::vector<ApplicationWindow*>();

				// Add only the windows that are still open to the list
				// Dispose all buttons of windows that were removed
				for (int i = 0; i < applicationWindowList.size(); i++)
				{
					bool exists = false;
					for (int j = 0; j < applicationHandleList->size(); j++)
						if (applicationWindowList[i]->HandleMatches((*applicationHandleList)[j]))
							exists = true;
					if (exists)
						newApplicationWindowList.push_back(applicationWindowList[i]);
					else
						applicationWindowList[i]->Destroy();
				}

				// NIY: Update systray and quick-launch

				// NIY: Put start button and quick-launch in the correct place

				// NIY: Actually calculate something
				// Calculate available space in taskbar for application windows and divide it over the buttons
				startingPositionX = startWindow->GetRect().right + 2;
				applicationWindowWidth = IsPrimary ? 160 : 22;

				// NIY: Moving stuff around
				// Re-display all windows
				for (int i = 0; i < newApplicationWindowList.size(); i++)
				{
					SIZE size{0, 0};
					Button_GetIdealSize(startWindow->WindowHandle, &size);
					newApplicationWindowList[i]->SetSize(applicationWindowWidth, size.cy);
					//newApplicationWindowList[i]->SetSize(applicationWindowWidth, 22);

					newApplicationWindowList[i]->SetPosition(startingPositionX + (applicationWindowWidth * i), 4);
					// TODO: Move this into ApplicationWindow???
					newApplicationWindowList[i]->SetFont(font);
					newApplicationWindowList[i]->SetText(newApplicationWindowList[i]->Info.GetTitle());


					newApplicationWindowList[i]->SetIcon(newApplicationWindowList[i]->Info.GetIcon());
					newApplicationWindowList[i]->SetParent(WindowHandle);
					if (!IsWindowVisible(newApplicationWindowList[i]->WindowHandle))
						ShowWindow(newApplicationWindowList[i]->WindowHandle, SW_SHOW);
				}

				// Free the old list and set the old reference to the new list
				//delete& applicationWindowList;
				applicationWindowList = newApplicationWindowList;

				// Remove old handle map
				applicationWindowHandleMap.clear();

				// Map all button handles to the button objects themselves
				for (int i = 0; i < applicationWindowList.size(); i++)
				{
					applicationWindowHandleMap[applicationWindowList[i]->WindowHandle] = applicationWindowList[i];
				}

				break;
			case WM_COMMAND:
				switch (HIWORD(wParam))
				{
				case BN_CLICKED:
					switch (LOWORD(wParam))
					{
					case IDC_APPLICATION_WINDOW:
						ApplicationWindow* button = applicationWindowHandleMap[(HWND)lParam];
						button->ClickHandler();
						break;
					}
					break;
				}
				break;
			case WM_DRAWITEM:
				dis = (LPDRAWITEMSTRUCT)lParam;
				
				hdc = dis->hDC;
				rc = dis->rcItem;

				switch (dis->CtlID)
				{
				case IDC_APPLICATION_WINDOW:
					HFONT oldFont;
					SIZE buttonSize;
					RECT textRect;
					ApplicationWindow* button = applicationWindowHandleMap[dis->hwndItem];
					if (!button)
						return DefWindowProc(hwnd, msg, wParam, lParam);

					buttonSize = button->GetSize();

					// Draw button frame
					//if (dis->itemAction == ODA_DRAWENTIRE)
					bool isPushed = dis->itemState & ODS_SELECTED;
					DrawFrameControl(hdc, &rc, DFC_BUTTON, DFCS_BUTTONPUSH | (isPushed ? DFCS_PUSHED : NULL));
					
					// Draw the icon
					// TODO: dpi aware icon size
					RECT temp = isPushed ? RECT{ 5, 3, 5 + 16, 3 + 16 } : RECT{ 4, 3, 4 + 16, 3 + 16 };
					if (DEBUG_DRAWBG)
						FillRect(hdc, &temp, GetSysColorBrush(COLOR_GRADIENTACTIVECAPTION));
					DrawIconEx(hdc, isPushed ? 5 : 4, isPushed ? 4 : 3, button->GetIcon(true), 16, 16, NULL, NULL, DI_NORMAL);
					
					// Draw the text
					textRect = isPushed ? RECT{ 23, 1, buttonSize.cx - 2, buttonSize.cy - 2 } : RECT{ 22, 1, buttonSize.cx - 2, buttonSize.cy - 2 };
					if (DEBUG_DRAWBG)
						FillRect(hdc, &textRect, GetSysColorBrush(COLOR_DESKTOP));
					oldFont = (HFONT)SelectObject(hdc, font);
					SetTextColor(hdc, GetSysColor(COLOR_BTNTEXT));
					DrawTextEx(hdc, button->GetText(), -1, &textRect, DT_VCENTER | DT_SINGLELINE | DT_END_ELLIPSIS | DT_LEFT, NULL);
					SelectObject(hdc, oldFont);
					
					break;
				}

				// Paint line
				// Graphics graphics(hdc);
				// Pen pen(Color(GetSysColor(COLOR_BTNHIGHLIGHT) + 0xFF000000));
				// graphics.DrawLine(&pen, 0, 1, 1280, 1);

				// EndPaint(hwnd, &ps);

				break;
			case WM_PAINT:
				hdc = GetDC(hwnd);
				GetClientRect(hwnd, &rc);
				// Paint line
				Graphics graphics(hdc);
				Pen pen(Color(GetSysColor(COLOR_BTNHIGHLIGHT) + 0xFF000000));
				graphics.DrawLine(&pen, 0, 1, 1280, 1);

				SwapBuffers(hdc);
				ReleaseDC(hwnd, hdc);
				break;

				//hdc = BeginPaint(hwnd, &ps);
				//GetClientRect(hwnd, &rc);

				//// Paint line
				//Graphics graphics(hdc);
				//Pen pen(Color(GetSysColor(COLOR_BTNHIGHLIGHT) + 0xFF000000));
				//graphics.DrawLine(&pen, 0, 1, 1280, 1);

				//EndPaint(hwnd, &ps);
				//break;
			}

			return DefWindowProc(hwnd, msg, wParam, lParam);
		}
	}
}