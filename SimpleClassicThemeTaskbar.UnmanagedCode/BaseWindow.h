#pragma once
#include "UnmanagedCode.h"
#include "resource.h"
#include <unordered_map>

#define IDC_GENERIC_BUTTON			3000
#define IDC_APPLICATION_WINDOW		3001
#define IDC_START_WINDOW			3002
#define IDC_TASKLIST				3003

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class Window
		{
		public:
			Window();
			~Window();

			void RegisterWindowClass();
			void CreateWindowHandle();
			void Destroy();

			HWND GetParent();
			RECT GetRect();
			POINT GetPosition();
			SIZE GetSize();
			TCHAR* GetText();
			HFONT GetFont();

			void SetParent(HWND parentHandle);
			void SetPosition(int x, int y);
			void SetPosition(POINT point);
			void SetSize(int cX, int cY);
			void SetSize(SIZE size);
			void SetText(TCHAR* text);
			void SetFont(HFONT font);

			HWND WindowHandle;
		protected:
			HWND parent;
			CREATESTRUCT createParams;
		private:
			virtual LRESULT CALLBACK windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
			static LRESULT CALLBACK staticWindowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
		};
	}
}
