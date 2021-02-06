#include "Win32Window.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		Win32Window::Win32Window() : Window()
		{

		}

		Win32Window::~Win32Window()
		{

		}

		void Win32Window::SetParent(HWND parentHandle)
		{
			if (!WindowHandle)
			{
				createParams.hwndParent = parentHandle;
				CreateWindowHandle();
			}
			else
			{
				Window::SetParent(parentHandle);
			}
		}
	}
}