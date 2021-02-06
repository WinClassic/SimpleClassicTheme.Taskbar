#pragma once
#include "Win32Window.h"
#include "ApplicationWindow.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		class TaskListWindow : public Win32Window
		{
		public:
			TaskListWindow();
			~TaskListWindow();

			int GetItemCount();
			void GetItem(int index, LPTCITEM item);
			HIMAGELIST GetImageList();

			void InsertItem(LPTCITEM item);
			void DeleteItem(int index);

			BOOL SetItem(int index, LPTCITEM item);
			HIMAGELIST SetImageList(HIMAGELIST newList);
			int SetItemSize(LPSIZE size);

			void Update(std::vector<HWND>* handleList);
		};
	}
}