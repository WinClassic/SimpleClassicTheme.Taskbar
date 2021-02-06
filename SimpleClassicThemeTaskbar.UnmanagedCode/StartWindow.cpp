#include "StartWindow.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		StartWindow::StartWindow() : ButtonWindow()
		{
			createParams.hMenu = (HMENU)IDC_START_WINDOW;
			createParams.lpszName = L"Start";
			createParams.style |= BS_TOP | BS_BOTTOM;
		}

		StartWindow::~StartWindow() { }
	}
}
