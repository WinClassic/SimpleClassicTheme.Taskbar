#include "windows.h"
#include "appmodel.h"
#include "Shobjidl.h"
#pragma once

#pragma comment( lib, "Ole32.lib" )

namespace SimpleClassicThemeTaskbar
{
	
    // This is our native implementation
    // It's marked with __declspec(dllexport) 
    // to be visible from outside the DLL boundaries
    class __declspec(dllexport) UnmanagedCode
    {
    public:
		WCHAR* GetAppUserModelId(int pid);
		void SetWorkingArea(int left, int right, int top, int bottom, bool sendChange, HWND* windows, int count);
		void InitCom();
		int GetSize(HWND hWnd);
		void DeInitCom();
        bool WindowIsOnCurrentDesktop(HWND hWnd) const;
		int GetTrayButtonCount(HWND sysTray);
		int UnmanagedSCTT();

		struct TRAYBUTTONINFO
		{
			HWND hwnd;
			DWORD pid;
			wchar_t* toolTip;
			bool visible;
			HICON icon;
			unsigned int callbackMessage;
			unsigned int id;
		};
		TRAYBUTTONINFO GetTrayButton(HWND sysTray, int i);
		TRAYBUTTONINFO* GetTrayButtons(HWND sysTray, int count);
    };
}