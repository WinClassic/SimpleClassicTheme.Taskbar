#pragma once

#include "BaseComponentRenderer.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ClassicComponentRenderer : public ComponentRenderer
		{
		public:
			void Initialize();
			void DrawTaskbar(HWND hWnd);
			void DrawStartButton(StartWindow* window, PDRAWITEMSTRUCT drawStruct);
			void DrawTaskList(TaskListWindow* window, PDRAWITEMSTRUCT drawStruct);
			void Destroy();
		};
	}
}

