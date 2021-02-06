#pragma once
#include "Taskbar.h"
#include <vector>

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		static class BackgroundThread
		{
		public:
			std::vector<Taskbar> Taskbars;
		};
	}
}

