#pragma once

namespace SimpleClassicThemeTaskbar
{
	namespace Cpp
	{
		class Interop;

		namespace CLI
		{
			public ref struct TBUTTONINFO
			{
				System::IntPtr hwnd;
				unsigned long pid;
				System::String^ toolTip;
				bool visible;
				System::IntPtr icon;
				unsigned int callbackMessage;
				unsigned int id;
			};
			
			public ref class Interop
			{
			public:
				Interop();
				~Interop();
				!Interop();

				void InitCom();
				void DeInitCom();
				int GetSize(System::IntPtr hWnd);
				bool WindowIsOnCurrentDesktop(System::IntPtr hWnd);
				int GetTrayButtonCount(System::IntPtr sysTray);
				bool GetTrayButton(System::IntPtr sysTray, int i, TBUTTONINFO^% button);
				void SetWorkingArea(int left, int right, int top, int bottom);
				void Destroy();
			private:
				Cpp::Interop* _impl;
			};
		}
	}
}