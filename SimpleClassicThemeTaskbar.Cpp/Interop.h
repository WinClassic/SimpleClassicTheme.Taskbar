#pragma once

namespace SimpleClassicThemeTaskbar
{
	namespace Cpp
	{
		class Interop;

		namespace CLI
		{
			public ref class Interop
			{
			public:
				Interop();
				~Interop();
				!Interop();

				void InitCom();
				void DeInitCom();
				bool WindowIsOnCurrentDesktop(System::IntPtr hWnd);

				void Destroy();
			private:
				Cpp::Interop* _impl;
			};
		}
	}
}