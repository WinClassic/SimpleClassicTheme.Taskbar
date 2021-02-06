#include "..\SimpleClassicThemeTaskbar.UnmanagedCode\UnmanagedCode.h"
#include "CodeBridge.h"

using namespace std;

System::String^ SimpleClassicThemeTaskbar::CodeBridge::GetAppUserModelId(int pid)
{
	return gcnew System::String(_impl->GetAppUserModelId(pid));
}

void SimpleClassicThemeTaskbar::CodeBridge::SetWorkingArea(int left, int right, int top, 
	int bottom, bool sendChange, array<System::IntPtr>^ windows)
{
	int length = windows->Length;
	if (length > 0)
	{
		HWND* hwnds = (HWND*)malloc(sizeof(HWND) * length);
		for (int i = 0; i < length; i++)
		{
			System::IntPtr ptr = (System::IntPtr)windows->GetValue(i);
			HWND hwnd = (HWND)ptr.ToPointer();
			hwnds[i] = hwnd;
		}
		_impl->SetWorkingArea(left, right, top, bottom, sendChange, hwnds, length);
		free(hwnds);
	}
	else
	{
		_impl->SetWorkingArea(left, right, top, bottom, sendChange, NULL, length);
	}
}

SimpleClassicThemeTaskbar::CodeBridge::CodeBridge()
	: _impl(new UnmanagedCode())
	// Allocate some memory for the native implementation
{
}

int SimpleClassicThemeTaskbar::CodeBridge::GetSize(System::IntPtr wnd)
{
	return _impl->GetSize((HWND) wnd.ToPointer());
}

void SimpleClassicThemeTaskbar::CodeBridge::InitCom()
{
	_impl->InitCom();
}

void SimpleClassicThemeTaskbar::CodeBridge::DeInitCom()
{
	_impl->DeInitCom();
}

bool SimpleClassicThemeTaskbar::CodeBridge::WindowIsOnCurrentDesktop(System::IntPtr wnd)
{
	return _impl->WindowIsOnCurrentDesktop((HWND) wnd.ToPointer()); // Call native Get
}

int SimpleClassicThemeTaskbar::CodeBridge::GetTrayButtonCount(System::IntPtr sysTray)
{
	return _impl->GetTrayButtonCount((HWND) sysTray.ToPointer());
}

int SimpleClassicThemeTaskbar::CodeBridge::UnmanagedSCTT()
{
	return _impl->UnmanagedSCTT();
}

bool SimpleClassicThemeTaskbar::CodeBridge::GetTrayButton(System::IntPtr sysTray, int i, TBUTTONINFO^% button)
{
	UnmanagedCode::TRAYBUTTONINFO q = _impl->GetTrayButton(HWND(sysTray.ToPointer()), i);
	if (q.hwnd == nullptr)
		return false;
	button->hwnd = System::IntPtr(q.hwnd);
	button->icon = System::IntPtr(q.icon);
	button->pid = q.pid;
	button->toolTip = gcnew System::String(q.toolTip);
	button->visible = q.visible;
	button->callbackMessage = q.callbackMessage;
	button->id = q.id;
	return true;
}

void SimpleClassicThemeTaskbar::CodeBridge::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

SimpleClassicThemeTaskbar::CodeBridge::~CodeBridge()
{
	// C++ CLI compiler will automaticly make all ref classes implement IDisposable.
	// The default implementation will invoke this method + call GC.SuspendFinalize.
	Destroy(); // Clean-up any native resources 
}

SimpleClassicThemeTaskbar::CodeBridge::!CodeBridge()
{
	// This is the finalizer
	// It's essentially a fail-safe, and will get called
	// in case Logic was not used inside a using block.
	Destroy(); // Clean-up any native resources 
}

