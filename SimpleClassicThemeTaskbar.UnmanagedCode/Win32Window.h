#pragma once
#include "BaseWindow.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class Win32Window : public Window
		{
		public:
			Win32Window();
			~Win32Window();

			void SetParent(HWND parentHandle);
		};
	}
}
