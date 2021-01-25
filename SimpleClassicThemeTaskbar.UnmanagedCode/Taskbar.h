#pragma once
#include "windows.h"

namespace SimpleClassicThemeTaskbar
{
	namespace UnmanagedCode 
	{
		class __declspec(dllexport) Window
		{

		};

		class __declspec(dllexport) Taskbar
		{
		public:
			Taskbar();
			~Taskbar();

			void RegisterWindowClass();

			void Create();
			void ShowOnScreen(RECT workArea);
		private:
			static bool windowClassRegistered;
			static LRESULT CALLBACK windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
			
		};
	}
}

