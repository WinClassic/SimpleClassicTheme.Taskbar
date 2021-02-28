#pragma once

#include "StartWindow.h"
#include "TaskListWindow.h"

#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ComponentRenderer
		{
		public:
			virtual void Initialize() = 0;
			virtual void DrawTaskbar(HWND hWnd) = 0;
			virtual void DrawStartButton(StartWindow* window, PDRAWITEMSTRUCT drawStruct) = 0;
			virtual void DrawTaskList(TaskListWindow* window, PDRAWITEMSTRUCT drawStruct) = 0;
			virtual void Destroy() = 0;
		};
	}
}

