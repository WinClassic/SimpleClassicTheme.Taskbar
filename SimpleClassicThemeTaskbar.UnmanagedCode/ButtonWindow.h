#pragma once
#include "Win32Window.h"
#include <vector>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ButtonWindow : public Win32Window
		{
		public:
			ButtonWindow();
			~ButtonWindow();

			void SetIcon(HICON hicon);

			HICON GetIcon(bool drawnIcon);

		private:
			HICON originalIcon;
			HICON displayIcon;
			//void CreateWindowHandle(HWND parent);
		};
	}
}


