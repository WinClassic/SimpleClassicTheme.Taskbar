#include "windows.h"
#include "Shobjidl.h"
#pragma once

#pragma comment( lib, "Ole32.lib" )

namespace SimpleClassicThemeTaskbar
{
    namespace Cpp
    {
        // This is our native implementation
        // It's marked with __declspec(dllexport) 
        // to be visible from outside the DLL boundaries
        class __declspec(dllexport) Interop
        {
        public:
			void InitCom();
			void DeInitCom();
            bool WindowIsOnCurrentDesktop(HWND hWnd) const;
        };
    }
}