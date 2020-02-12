#include "..\SimpleClassicThemeTaskbar.Cpp.Interop\Interop.h"
#include "Interop.h"

using namespace std;

SimpleClassicThemeTaskbar::Cpp::CLI::Interop::Interop()
	: _impl(new Cpp::Interop())
	// Allocate some memory for the native implementation
{
}

int SimpleClassicThemeTaskbar::Cpp::CLI::Interop::GetSize(System::IntPtr wnd)
{
	return _impl->GetSize((HWND) wnd.ToPointer());
}

void SimpleClassicThemeTaskbar::Cpp::CLI::Interop::InitCom()
{
	_impl->InitCom();
}

void SimpleClassicThemeTaskbar::Cpp::CLI::Interop::DeInitCom()
{
	_impl->DeInitCom();
}

bool SimpleClassicThemeTaskbar::Cpp::CLI::Interop::WindowIsOnCurrentDesktop(System::IntPtr wnd)
{
	return _impl->WindowIsOnCurrentDesktop((HWND) wnd.ToPointer()); // Call native Get
}

int SimpleClassicThemeTaskbar::Cpp::CLI::Interop::GetTrayButtonCount(System::IntPtr sysTray)
{
	return _impl->GetTrayButtonCount((HWND) sysTray.ToPointer());
}

bool SimpleClassicThemeTaskbar::Cpp::CLI::Interop::GetTrayButton(System::IntPtr sysTray, int i, TBUTTONINFO^% button)
{
	TRAYBUTTONINFO q = _impl->GetTrayButton(HWND(sysTray.ToPointer()), i);
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

void SimpleClassicThemeTaskbar::Cpp::CLI::Interop::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

SimpleClassicThemeTaskbar::Cpp::CLI::Interop::~Interop()
{
	// C++ CLI compiler will automaticly make all ref classes implement IDisposable.
	// The default implementation will invoke this method + call GC.SuspendFinalize.
	Destroy(); // Clean-up any native resources 
}

SimpleClassicThemeTaskbar::Cpp::CLI::Interop::!Interop()
{
	// This is the finalizer
	// It's essentially a fail-safe, and will get called
	// in case Logic was not used inside a using block.
	Destroy(); // Clean-up any native resources 
}

