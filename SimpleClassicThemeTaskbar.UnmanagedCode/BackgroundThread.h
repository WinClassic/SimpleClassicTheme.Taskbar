#pragma once
#include "Taskbar.h"
#include <vector>

namespace SimpleClassicThemeTaskbar
{
	namespace UnmanagedCode 
	{
		static class BackgroundThread
		{
		public:
			std::vector<Taskbar> Taskbars;
		};
	}
}

