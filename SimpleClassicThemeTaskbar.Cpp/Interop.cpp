#include "..\SimpleClassicThemeTaskbar.Cpp.Interop\Interop.h"
#include "Interop.h"

using namespace std;

SimpleClassicThemeTaskbar::Cpp::CLI::Interop::Interop()
	: _impl(new Cpp::Interop())
	// Allocate some memory for the native implementation
{
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

