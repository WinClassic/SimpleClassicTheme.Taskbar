#pragma once

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class ComponentRenderer
		{
			virtual void DrawStartButton() abstract;
			virtual void DrawTaskbar() abstract;
			virtual void Draw() abstract;
			virtual void DrawButton() abstract;
		};
	}
}

