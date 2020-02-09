#include "Interop.h"

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