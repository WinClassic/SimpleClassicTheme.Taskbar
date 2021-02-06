#include "BaseWindow.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		Window::Window()
		{
			createParams = { 0 };
			createParams.lpszClass = L"SCTT_Generic";
			createParams.lpszName = L"SCT Generic Window";
			createParams.hInstance = GetModuleHandle(NULL);
			createParams.lpCreateParams = this;
			createParams.x = CW_USEDEFAULT;
			createParams.y = CW_USEDEFAULT;
			createParams.cx = CW_USEDEFAULT;
			createParams.cy = CW_USEDEFAULT;

			WindowHandle = NULL;
			parent = NULL;
		}

		Window::~Window() { if (WindowHandle) DestroyWindow(WindowHandle); }

		//Register the window class for this Window and all future Window objects
		void Window::RegisterWindowClass()
		{
			WNDCLASSEX wc = { };
			wc.cbSize = sizeof(WNDCLASSEX);
			wc.style = NULL;
			wc.lpfnWndProc = staticWindowProcedure;
			wc.cbClsExtra = NULL;
			wc.cbWndExtra = NULL;
			wc.hInstance = GetModuleHandle(NULL);
			wc.hIcon = NULL;
			wc.hCursor = LoadCursor(NULL, IDC_ARROW);
			wc.hbrBackground = GetSysColorBrush(COLOR_BTNFACE);
			wc.lpszMenuName = NULL;
			wc.lpszClassName = createParams.lpszClass;
			wc.hIconSm = NULL;

			if (RegisterClassEx(&wc) == NULL)
				DebugBreak();
		}

		void Window::CreateWindowHandle()
		{
			//Create instance of the window class
			WindowHandle = CreateWindowExW(createParams.dwExStyle,
				createParams.lpszClass, 
				createParams.lpszName,
				createParams.style,
				createParams.x,
				createParams.y,
				createParams.cx,
				createParams.cy,
				createParams.hwndParent,
				createParams.hMenu,
				createParams.hInstance,
				createParams.lpCreateParams);
			if (!WindowHandle)
//#ifdef _DEBUG
				DebugBreak();
//#endif
				//exit(GetLastError());
		}

		void Window::Destroy()
		{
			DestroyWindow(WindowHandle);
			WindowHandle = NULL;
		}

		HWND Window::GetParent()
		{
			return ::GetParent(WindowHandle);
		}

		RECT Window::GetRect()
		{
			RECT windowRect;
			GetWindowRect(WindowHandle, &windowRect);
			return windowRect;
		}

		POINT Window::GetPosition()
		{
			RECT windowRect = GetRect();
			return POINT{ windowRect.left, windowRect.top };
		}

		SIZE Window::GetSize()
		{
			RECT windowRect = GetRect();
			return SIZE{ windowRect.right - windowRect.left, windowRect.bottom - windowRect.top };
		}

		TCHAR* Window::GetText()
		{
			TCHAR* text = (TCHAR*)malloc(sizeof(TCHAR) * 100);
			GetWindowText(WindowHandle, text, 100);
			return text;
		}

		HFONT Window::GetFont()
		{
			return (HFONT)SendMessage(WindowHandle, WM_GETFONT, NULL, NULL);
		}

		void Window::SetParent(HWND parentHandle)
		{
			if (GetParent() != parentHandle)
				::SetParent(WindowHandle, parentHandle);
		}

		void Window::SetPosition(int x, int y)
		{
			return SetPosition(POINT{ x, y });
		}

		void Window::SetPosition(POINT point)
		{
			POINT originalPosition = GetPosition();
			if (originalPosition.x == point.x && originalPosition.y == point.y)
				return;
			SetWindowPos(WindowHandle, NULL, point.x, point.y, 0, 0, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER | SWP_NOSIZE);
		}

		void Window::SetSize(int cX, int cY)
		{
			return SetSize(SIZE{ cX, cY });
		}

		void Window::SetSize(SIZE size)
		{
			SIZE originalSize = GetSize();
			if (originalSize.cx == size.cx && originalSize.cy == size.cy)
				return;
			SetWindowPos(WindowHandle, NULL, 0, 0, size.cx, size.cy, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER | SWP_NOMOVE);
		}

		void Window::SetText(TCHAR* text)
		{
			SetWindowText(WindowHandle, text);
		}

		void Window::SetFont(HFONT font)
		{
			SendMessage(WindowHandle, WM_SETFONT, (WPARAM)font, TRUE);
		}

		//Window procedure for this Window
		LRESULT CALLBACK Window::windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
		{
			return DefWindowProc(hwnd, msg, wParam, lParam);
		}

		//Main window procedure for all windows
		LRESULT CALLBACK Window::staticWindowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
		{
			Window* instance;

			//Retrieve pointer to current instance of the Window object
			if (msg == WM_NCCREATE)
			{
				instance = static_cast<Window*>(reinterpret_cast<CREATESTRUCT*>(lParam)->lpCreateParams);

				SetLastError(0);
				if (!SetWindowLongPtr(hwnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(instance)))
				{
					if (GetLastError() != 0)
						return FALSE;
				}
			}
			else
			{
				instance = reinterpret_cast<Window*>(GetWindowLongPtr(hwnd, GWLP_USERDATA));
			}

			if (instance)
			{
				return instance->windowProcedure(hwnd, msg, wParam, lParam);
			}

			return DefWindowProc(hwnd, msg, wParam, lParam);
		}
	}
}