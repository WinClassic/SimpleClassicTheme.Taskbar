#include "Interop.h"
#include "ProcessData.h"

using namespace std;

IVirtualDesktopManager* g_pvdm;

void SimpleClassicThemeTaskbar::Cpp::Interop::InitCom()
{
	if (SUCCEEDED(CoInitialize(NULL)))
	{
		/* In case we use COM */
		//Get a pointer to the Virtual Desktop Manager
		CoCreateInstance(CLSID_VirtualDesktopManager,
			nullptr, CLSCTX_ALL, IID_PPV_ARGS(&g_pvdm));
	}
}

void SimpleClassicThemeTaskbar::Cpp::Interop::DeInitCom()
{
	//Release the COM object
	if (g_pvdm) g_pvdm->Release();
}

//Gets a size from a window handle
int SimpleClassicThemeTaskbar::Cpp::Interop::GetSize(HWND hWnd)
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
bool SimpleClassicThemeTaskbar::Cpp::Interop::WindowIsOnCurrentDesktop(HWND hWnd) const
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

int SimpleClassicThemeTaskbar::Cpp::Interop::GetTrayButtonCount(HWND sysTray)
{
	return SendMessage(sysTray, TB_BUTTONCOUNT, 0, 0);
}

struct TRAYDATA
{
	HWND hwnd;
	UINT uID;
	UINT uCallbackMessage;
	DWORD Reserved[2];
	HICON hIcon;
};


SimpleClassicThemeTaskbar::Cpp::TRAYBUTTONINFO SimpleClassicThemeTaskbar::Cpp::Interop::GetTrayButton(HWND sysTray, int i)
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

	//Return the info struct
	return tbinfo;
}