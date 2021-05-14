#include "ClassicComponentRenderer.h"

namespace SimpleClassicThemeTaskbar
{
	namespace Unmanaged
	{
		void ClassicComponentRenderer::Initialize()
		{

		}

		void ClassicComponentRenderer::DrawTaskbar(HWND hWnd)
		{
			HDC hDC;
			RECT rc;
			hDC = GetDC(hWnd);
			GetClientRect(hWnd, &rc);
			Rect rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
			Graphics graphics(hDC);

			// Fill background
			SolidBrush brush(Color(GetSysColor(COLOR_MENU) + 0xFF000000));
			graphics.FillRectangle(&brush, rect);

			// Paint line
			Pen pen(Color(GetSysColor(COLOR_BTNHIGHLIGHT) + 0xFF000000));
			graphics.DrawLine(&pen, 0, 1, rect.Width, 1);

			SwapBuffers(hDC);
			ReleaseDC(hWnd, hDC);
		}

		void ClassicComponentRenderer::DrawStartButton(StartWindow* window, PDRAWITEMSTRUCT lpDrawItemStruct)
		{

		}

		void ClassicComponentRenderer::DrawTaskList(TaskListWindow* window, PDRAWITEMSTRUCT lpDrawItemStruct)
		{
			RECT rect = lpDrawItemStruct->rcItem;
			int nTabIndex = lpDrawItemStruct->itemID;
			if (nTabIndex < 0) return;
			BOOL bSelected = (nTabIndex == window->GetCurSel());

			TCHAR label[128];
			TC_ITEM tci;
			tci.mask = TCIF_TEXT | TCIF_IMAGE;
			tci.pszText = label;
			tci.cchTextMax = 127;
			if (!window->GetItem(nTabIndex, &tci)) return;

			HDC hDC = lpDrawItemStruct->hDC;
			if (!hDC) return;
			int nSavedDC = SaveDC(hDC);

			//rect.top += GetSystemMetrics(SM_CYEDGE);
			SetBkMode(hDC, TRANSPARENT);
			FillRect(hDC, &rect, GetSysColorBrush(COLOR_MENUHILIGHT));
			//DrawFrameControl(hDC, &rect, DFC_BUTTON, DFCS_BUTTONPUSH | (bSelected ? DFCS_PUSHED : 0));



			RestoreDC(hDC, nSavedDC);
		}

		void ClassicComponentRenderer::Destroy()
		{

		}
	}
}
