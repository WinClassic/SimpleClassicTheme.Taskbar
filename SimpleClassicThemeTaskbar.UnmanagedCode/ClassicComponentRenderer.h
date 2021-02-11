#pragma once

#include "BaseComponentRenderer.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ClassicComponentRenderer : ComponentRenderer
		{
			void Initialize();
			void DrawStartButton(StartWindow window);
			void DrawTaskbar(Taskbar taskbar);
			void DrawTaskList(TaskListWindow window);
			void Destroy();
		};
	}
}

