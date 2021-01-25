#include "Taskbar.h"

namespace SimpleClassicThemeTaskbar
{
	namespace UnmanagedCode
	{
		class Taskbar
		{
			const wchar_t className[19] = L"SCTT_Shell_TrayWnd";
			bool windowClassRegistered = false;
			
			//Constructor
			Taskbar()
			{
				//Check if the class is registered
				if (!windowClassRegistered)
					RegisterWindowClass();


			}

			//Register the window class for all Taskbar objects
			void RegisterWindowClass()
			{
				WNDCLASS wc = { };
				wc.lpfnWndProc = windowProcedure;
				wc.hInstance = GetModuleHandle(NULL);
				wc.lpszClassName = className;

				RegisterClass(&wc);
			}

			//Main window procedure for taskbars
			static LRESULT CALLBACK windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
			{
				Taskbar* instance;

				//Retrieve pointer to current instance of the Taskbar object
				if (msg == WM_NCCREATE)
				{
					instance = static_cast<Taskbar*>(reinterpret_cast<CREATESTRUCT*>(lParam)->lpCreateParams);

					SetLastError(0);
					if (!SetWindowLongPtr(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(instance)))
					{
						if (GetLastError() != 0)
							return FALSE;
					}
				}
				else
				{
					instance = reinterpret_cast<Taskbar*>(GetWindowLongPtr(hwnd, GWLP_USERDATA));
				}

				if (instance)
				{

				}

				return DefWindowProc(hwnd, msg, wParam, lParam);
			}
		};
	}
}