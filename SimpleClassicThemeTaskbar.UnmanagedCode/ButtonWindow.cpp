#include "ButtonWindow.h"
#include <wingdi.h>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		ButtonWindow::ButtonWindow() : Win32Window()
		{
			originalIcon = NULL;
			displayIcon = NULL;

			createParams.lpszClass = WC_BUTTON;
			createParams.style = WS_VISIBLE | WS_CHILD | BS_PUSHBUTTON | BS_LEFT;
			createParams.hMenu = (HMENU)IDC_GENERIC_BUTTON;
			createParams.hInstance = NULL;
		}

		ButtonWindow::~ButtonWindow()
		{
			//DestroyWindow(Info.WindowHandle);
		}

		void ButtonWindow::SetIcon(HICON hicon)
		{
			if (!WindowHandle)
				return;

			HICON newIcon;

			if (originalIcon != hicon)
			{
				originalIcon = hicon;
				ICONINFO iconInfo = { 0 };
				GetIconInfo(originalIcon, &iconInfo);
				// TODO: Dpi aware icon constant
				newIcon = (HICON)CopyImage(originalIcon, IMAGE_ICON, 16, 16, NULL);
				if (newIcon)
				{
					SendMessage(WindowHandle, BM_SETIMAGE, IMAGE_ICON, (LPARAM)newIcon);
					DestroyIcon(displayIcon);
					displayIcon = newIcon;
				}
			}
		}

		HICON ButtonWindow::GetIcon(bool drawnIcon)
		{
			if (drawnIcon)
				return displayIcon;
			else
				return originalIcon;
		}
	}
}