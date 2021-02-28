#include "TaskListWindow.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		TaskListWindow::TaskListWindow() : Win32Window()
		{
			createParams.lpszClass = WC_TABCONTROL;
			createParams.style = WS_VISIBLE | WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | TCS_BUTTONS | TCS_FIXEDWIDTH | TCS_MULTILINE | TCS_FOCUSNEVER;
			createParams.style = WS_VISIBLE | WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS /*| TCS_BUTTONS*/ | TCS_FIXEDWIDTH | TCS_MULTILINE | TCS_FOCUSNEVER | TCS_OWNERDRAWFIXED;
			createParams.hMenu = (HMENU)IDC_TASKLIST;
		}
		
		TaskListWindow::~TaskListWindow()
		{

		}

		int TaskListWindow::GetItemCount()
		{
			return TabCtrl_GetItemCount(WindowHandle);
		}

		BOOL TaskListWindow::GetItem(int index, LPTCITEM item)
		{
			return TabCtrl_GetItem(WindowHandle, index, item);
		}

		HIMAGELIST TaskListWindow::GetImageList()
		{
			return TabCtrl_GetImageList(WindowHandle);
		}

		int TaskListWindow::GetCurSel()
		{
			return TabCtrl_GetCurSel(WindowHandle);
		}

		BOOL TaskListWindow::SetItem(int index, LPTCITEM item)
		{
			return TabCtrl_SetItem(WindowHandle, index, item);
		}

		HIMAGELIST TaskListWindow::SetImageList(HIMAGELIST newList)
		{
			return TabCtrl_SetImageList(WindowHandle, newList);
		}

		int TaskListWindow::SetItemSize(LPSIZE size)
		{
			return TabCtrl_SetItemSize(WindowHandle, size->cx, size->cy);
		}

		void TaskListWindow::InsertItem(LPTCITEM item)
		{
			TabCtrl_InsertItem(WindowHandle, GetItemCount(), item);
		}

		void TaskListWindow::DeleteItem(int index)
		{
			TabCtrl_DeleteItem(WindowHandle, index);
		}

		void TaskListWindow::Update(std::vector<HWND>* handleList)
		{
			SIZE itemSize;
			HIMAGELIST imageList;
			TCITEM tcItem;

			itemSize = SIZE{160, GetSize().cy};
			SetItemSize(&itemSize);
			
			// TODO dpi aware icon size
			//imageList = ImageList_Create(GetSize().cy - 4, GetSize().cy - 4, ILC_COLOR32, 0, handleList->size());
			imageList = ImageList_Create(16, 16, ILC_COLOR32, 0, handleList->size());

			for (int i = 0; i < handleList->size(); i++)
			{
				ApplicationWindowInfo awi = ApplicationWindowInfo((*handleList)[i]);
				ImageList_ReplaceIcon(imageList, -1, awi.GetIcon());
				tcItem = { 0 };
				tcItem.mask = TCIF_TEXT | TCIF_IMAGE;
				tcItem.pszText = awi.GetTitle();
				tcItem.iImage = i;
				InsertItem(&tcItem);
			}

			imageList = SetImageList(imageList);
			ImageList_Destroy(imageList);
		}
	}
}