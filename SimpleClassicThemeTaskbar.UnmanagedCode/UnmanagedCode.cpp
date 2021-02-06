#include "Taskbar.h"
#include "UnmanagedCode.h"
#include "ProcessData.h"
#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")

using namespace std;

IVirtualDesktopManager* g_pvdm;

WCHAR* SimpleClassicThemeTaskbar::UnmanagedCode::GetAppUserModelId(int pid)
{
	HANDLE process = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
	if (process == NULL)
		return 0;
	WCHAR* buffer = (WCHAR*)malloc(sizeof(WCHAR) * 64);
	UINT32 size = 64U;
	unsigned long modelId = GetApplicationUserModelId(process, &size, buffer);
	if (modelId == APPMODEL_ERROR_NO_APPLICATION)
	{
		buffer = L"No user model id\0";
	}
	return buffer;
}

void SimpleClassicThemeTaskbar::UnmanagedCode::SetWorkingArea(int left, int right, int top, int bottom, bool sendChange, HWND* windows, int count) 
{
	RECT workarea = { 0 };

	//Modify workarea
	workarea.left = left;
	workarea.right = right;
	workarea.top = top;
	workarea.bottom = bottom;

	//Set the new work area and broadcast the change to all running applications
	if (!SystemParametersInfo(SPI_SETWORKAREA, 0, &workarea, sendChange ? SPIF_SENDCHANGE | SPIF_UPDATEINIFILE : 0))
	{
		LPWSTR error = 0;
		if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM, NULL, GetLastError(), 0, (LPWSTR)&error, 0, NULL) == 0);
			MessageBox(0, error, L"Could not change workarea", 0);
	}

	//Unmaximize and maximize any maximized applications
	if (count > 0)
	{
		for (int i = 0; i < count; i++)
		{
			HWND hwnd = windows[i];
			WINDOWPLACEMENT placement = { 0 };
			placement.length = sizeof(WINDOWPLACEMENT);
			GetWindowPlacement(hwnd, &placement);
			if (placement.showCmd == SW_SHOWMAXIMIZED)
			{
				ShowWindow(hwnd, SW_RESTORE);
				ShowWindow(hwnd, SW_MAXIMIZE);
			}
		}
	}
}

void SimpleClassicThemeTaskbar::UnmanagedCode::InitCom()
{
	if (SUCCEEDED(CoInitialize(NULL)))
	{
		/* In case we use COM */
		//Get a pointer to the Virtual Desktop Manager
		CoCreateInstance(CLSID_VirtualDesktopManager,
			nullptr, CLSCTX_ALL, IID_PPV_ARGS(&g_pvdm));
	}
}

void SimpleClassicThemeTaskbar::UnmanagedCode::DeInitCom()
{
	//Release the COM object
	if (g_pvdm) g_pvdm->Release();
}

//Gets a size from a window handle
int SimpleClassicThemeTaskbar::UnmanagedCode::GetSize(HWND hWnd)
{
	RECT rect;
	if (GetWindowRect(hWnd, &rect))
	{
		int width = rect.right - rect.left;
		int height = rect.bottom - rect.top;

		return (width << 16) + height;
	}
	return 0;
}

//Code partially sourced from:
//https://devblogs.microsoft.com/oldnewthing/20171002-00/?p=97116
bool SimpleClassicThemeTaskbar::UnmanagedCode::WindowIsOnCurrentDesktop(HWND hWnd) const
{
	if (g_pvdm)
	{
		//Ask the VDM if the window is on the current desktop
		BOOL isCurrent;
		if (SUCCEEDED(g_pvdm->IsWindowOnCurrentVirtualDesktop(hWnd, &isCurrent)))

		//Return the result from the VDM
		return isCurrent;
	}
	//COM failed so return true anyways
	//If we didn't get a pointer we're on OS < Win10 and
	//the window is probably on the current desktop
	return true;
}

int SimpleClassicThemeTaskbar::UnmanagedCode::GetTrayButtonCount(HWND sysTray)
{
	return SendMessage(sysTray, TB_BUTTONCOUNT, 0, 0);
}

int SimpleClassicThemeTaskbar::UnmanagedCode::UnmanagedSCTT()
{
	const POINT ptZero = { 0, 0 };
	HMONITOR monitor;
	MSG msg;
	BOOL bRet; 
	GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR           gdiplusToken;

	_CrtSetReportMode(_CRT_WARN, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_WARN, _CRTDBG_FILE_STDERR);
	_CrtSetReportMode(_CRT_ERROR, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_ERROR, _CRTDBG_FILE_STDERR);
	_CrtSetReportMode(_CRT_ASSERT, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_ASSERT, _CRTDBG_FILE_STDERR);

	//Initialize GDI+.
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	//Initialize a taskbar
	Unmanaged::Taskbar tb(true);

	//Get primary monitor working area and display the taskbar
	monitor = MonitorFromPoint(ptZero, MONITOR_DEFAULTTOPRIMARY);
	MONITORINFO monitorInfo = { 0 };
	monitorInfo.cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(monitor, &monitorInfo);
	tb.ShowOnScreen(monitorInfo.rcMonitor);

	//Start the message loop. 
	while ((bRet = GetMessage(&msg, NULL, 0, 0)) != 0)
	{
		if (bRet == -1)
		{
			//Handle the error and possibly exit
		}
		else
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	CloseHandle(monitor);

	return 0;
}

struct TRAYDATA
{
	HWND hwnd;
	UINT uID;
	UINT uCallbackMessage;
	DWORD Reserved[2];
	HICON hIcon;
};


SimpleClassicThemeTaskbar::UnmanagedCode::TRAYBUTTONINFO SimpleClassicThemeTaskbar::UnmanagedCode::GetTrayButton(HWND sysTray, int i)
{
	//Get explorer.exe PID
	DWORD sysTrayPid;
	GetWindowThreadProcessId(sysTray, &sysTrayPid);

	//Allocate memory
	CProcessData<TBBUTTON> data(sysTrayPid);

	//Initialize fields
	TBBUTTON tb = { 0 };
	TRAYDATA tray = { 0 };
	TRAYBUTTONINFO tbinfo = { 0 };

	//Get the button info
	::SendMessage(sysTray, TB_GETBUTTON, i, (LPARAM)data.GetData());
	data.ReadData(&tb);
	data.ReadData<TRAYDATA>(&tray, (LPCVOID)tb.dwData);

	//Get the tray icon process pid
	DWORD dwIconPid = 0;
	GetWindowThreadProcessId(tray.hwnd, &dwIconPid);

	//Get the tooltip
	wchar_t TipChar;
	wchar_t sTip[1024] = { 0 };
	wchar_t* pTip = (wchar_t*)tb.iString;
	if (!(tb.fsState & TBSTATE_HIDDEN))
	{
		int x = 0;
		do
		{
			if (x == 1023)
			{
				wcscpy(sTip, L"[ToolTip was either too long or not set]");
				break;
			}
			data.ReadData<wchar_t>(&TipChar, (LPCVOID)pTip++);
		} while (sTip[x++] = TipChar);
	}
	else
		wcscpy(sTip, L"[Hidden Icon]");

	//Save the info into the struct
	tbinfo.toolTip = sTip;
	tbinfo.hwnd = tray.hwnd;
	tbinfo.visible = !(tb.fsState & TBSTATE_HIDDEN);
	tbinfo.icon = tray.hIcon;
	tbinfo.callbackMessage = tray.uCallbackMessage;
	tbinfo.id = tray.uID;

	//Free process data handle
	data.~CProcessData();

	//Return the info struct
	return tbinfo;
}