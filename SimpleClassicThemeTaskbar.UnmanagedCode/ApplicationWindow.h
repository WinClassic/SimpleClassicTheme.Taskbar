#pragma once
#include "ButtonWindow.h"
#include <vector>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ApplicationWindowInfo
		{
		public: 
			ApplicationWindowInfo(HWND application);

			TCHAR* GetWindowClassName();
			TCHAR* GetTitle();
			WINDOWINFO GetWindowInfo();
			HICON GetIcon();

			HWND WindowHandle;
			unsigned long PID;
		};

		class ApplicationWindow : public ButtonWindow
		{
		public:
			ApplicationWindow(HWND application);
			~ApplicationWindow();

			void ClickHandler();

			void AddChildWindow(ApplicationWindow window);
			void RemoveChildWindow(ApplicationWindow window);

			bool IsGrouped;
			bool IsPartOfGroup;

			bool HandleMatches(HWND handle);

			ApplicationWindowInfo Info;

		private:
			std::vector<ApplicationWindow> children;
		};
	}
}
