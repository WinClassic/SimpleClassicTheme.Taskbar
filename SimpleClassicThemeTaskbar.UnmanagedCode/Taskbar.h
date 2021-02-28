#pragma once

#include "BaseComponentRenderer.h"
#include "UIComponents.h"

#include <vector>
#include <unordered_map>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged 
	{
		class Taskbar : public Window
		{
		public:
			Taskbar(bool isPrimary);
			~Taskbar();

			bool IsPrimary;

			void ShowOnScreen(RECT workArea);
		private:
			static std::vector<HWND>* windowsTaskbars;
			static std::vector<HWND>* secondaryTaskbars;
			static std::vector<HWND>* applicationHandleList;
			std::vector<ApplicationWindow*> applicationWindowList;
			std::unordered_map<HWND, ApplicationWindow*> applicationWindowHandleMap;

			StartWindow* startWindow;
			TaskListWindow* taskListWindow;
			ComponentRenderer* renderer;

			static bool windowClassRegistered;
			static bool secondaryWindowClassRegistered;
			static UnmanagedCode cppCode;
			static HFONT font;
			static HFONT boldFont;

			void DoAlternativeUpdate();
			
			static bool IsAltTabWindow(HWND hwnd, TCHAR* className);
			static BOOL CALLBACK enumWindowsProcedure(__in HWND hwnd, __in LPARAM lParam);
			LRESULT CALLBACK windowProcedure(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) override;
		};
	}
}

