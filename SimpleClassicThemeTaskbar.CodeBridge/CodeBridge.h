#pragma once

namespace SimpleClassicThemeTaskbar
{
	public ref class CodeBridge {	
		public:
			CodeBridge();
			~CodeBridge();
			!CodeBridge();

			void InitCom();
			void DeInitCom();
			int GetSize(System::IntPtr hWnd);
			bool WindowIsOnCurrentDesktop(System::IntPtr hWnd);
			int GetTrayButtonCount(System::IntPtr sysTray);
			int UnmanagedSCTT();

			ref struct TBUTTONINFO
			{
				System::IntPtr hwnd;
				unsigned long pid;
				System::String^ toolTip;
				bool visible;
				System::IntPtr icon;
				unsigned int callbackMessage;
				unsigned int id;
			};
			bool GetTrayButton(System::IntPtr sysTray, int i, TBUTTONINFO^% button);
			System::String^ GetAppUserModelId(int pid);
			void SetWorkingArea(int left, int right, int top, int bottom, bool sendChange, array<System::IntPtr>^ windows);
			void Destroy();
		private:
			UnmanagedCode* _impl;
	};
}