#pragma once

#include "UIComponents.h"

#include <gdiplus.h>
using namespace Gdiplus;
#pragma comment (lib,"Gdiplus.lib")

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ComponentRenderer
		{
			virtual void Initialize() abstract;
			virtual void DrawStartButton(StartWindow window) abstract;
			virtual void DrawTaskbar(Taskbar taskbar) abstract;
			virtual void DrawTaskList(TaskListWindow window) abstract;
			virtual void Destroy() abstract;
		};
	}
}

