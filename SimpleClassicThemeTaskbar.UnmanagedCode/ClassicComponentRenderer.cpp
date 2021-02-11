#include "ClassicComponentRenderer.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		void ClassicComponentRenderer::Initialize()
		{

		}

		void ClassicComponentRenderer::DrawStartButton(StartWindow window)
		{

		}

		void ClassicComponentRenderer::DrawTaskbar(Taskbar taskbar)
		{
			HDC hdc;
			RECT rc;

			hdc = GetDC(taskbar.WindowHandle);
			GetClientRect(taskbar.WindowHandle, &rc);
			// Paint line
			Graphics graphics(hdc);
			Pen pen(Color(GetSysColor(COLOR_BTNHIGHLIGHT) + 0xFF000000));
			graphics.DrawLine(&pen, 0, 1, 1280, 1);

			SwapBuffers(hdc);
			ReleaseDC(taskbar.WindowHandle, hdc);
		}

		void ClassicComponentRenderer::DrawTaskList(TaskListWindow window)
		{

		}

		void ClassicComponentRenderer::Destroy()
		{

		}
	}
}
