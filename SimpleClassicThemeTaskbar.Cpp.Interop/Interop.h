#include "windows.h"
#include "Shobjidl.h"
#pragma once

#pragma comment( lib, "Ole32.lib" )

namespace SimpleClassicThemeTaskbar
{
    namespace Cpp
    {
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
        // This is our native implementation
        // It's marked with __declspec(dllexport) 
        // to be visible from outside the DLL boundaries
        class __declspec(dllexport) Interop
        {
        public:
			void SimpleClassicThemeTaskbar::Cpp::Interop::SetWorkingArea(int left, int right, int top, int bottom);
			void InitCom();
			int GetSize(HWND hWnd);
			void DeInitCom();
            bool WindowIsOnCurrentDesktop(HWND hWnd) const;
			int GetTrayButtonCount(HWND sysTray);
			TRAYBUTTONINFO GetTrayButton(HWND sysTray, int i);
        };
    }
}