using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
	public class GroupedTaskbarProgram : BaseTaskbarProgram
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);
		
		const int MAX_PATH = 255;
		private static string GetShortPath(string path)
		{
			var shortPath = new StringBuilder(MAX_PATH);
			GetShortPathName(path, shortPath, MAX_PATH);
			return shortPath.ToString();
		}

		bool line = false;
		public GroupedTaskbarProgram()
		{
			Constructor();
			GroupWindow = new PopupTaskbarGroup(this);
			SpaceNeededNextToText = line ? 19 : 16;
		}

		public PopupTaskbarGroup GroupWindow;
		public List<SingleTaskbarProgram> ProgramWindows = new List<SingleTaskbarProgram>(); 
		public bool CanBeSingleWindow => ProgramWindows.Count < 2;

		public override Process Process { get => ProgramWindows[0].Process; set => ProgramWindows[0].Process = value; }
		public override Window Window { get => ProgramWindows[0].Window; set => ProgramWindows[0].Window = value; }
		public override string Title { get => ProgramWindows[0].Title; set => ProgramWindows[0].Title = value; }
		public override Icon Icon { get => ProgramWindows[0].Icon; set => ProgramWindows[0].Icon = value; }
		public override Image IconImage { get { try { return new Icon(Icon, 16, 16).ToBitmap(); } catch { return null; } } }

		public override string GetErrorString()
			=> GetBaseErrorString() +
			$"Process: {Process.MainModule.ModuleName} ({Process.Id})\n" +
			$"Window title: {Title}\n" +
			$"Window class: {Window.ClassName}\n" +
			$"Window HWND: {string.Join(", ", ProgramWindows.Select(o => o.Window.Handle.ToString("X8") + (IsWindow(o.Window.Handle) ? "Valid" : "Invalid")).ToArray())}\n" +
			$"Icon HWND: {string.Join(", ", ProgramWindows.Select(o => o.Icon.Handle.ToString("X8") + (IsWindow(o.Icon.Handle) ? "Valid" : "Invalid")).ToArray())}";

		public override bool IsActiveWindow(IntPtr activeWindow)
		{
			ApplicationEntryPoint.ErrorSource = this;
			controlState = "reordering group";
			foreach (SingleTaskbarProgram window in ProgramWindows)
			{
				window.Icon = Taskbar.GetAppIcon(window.Window);
				if (window.Window.Handle == activeWindow)
				{
					ActiveWindow = true;
					window.ActiveWindow = true;

					ProgramWindows.Remove(window);
					ProgramWindows.Insert(0, window);
					return true;
				}
				else
				{
					window.ActiveWindow = false;
				}
			}
			ActiveWindow = false;
			return false;
		}

		public override void FinishOnPaint(PaintEventArgs e)
		{
			ApplicationEntryPoint.ErrorSource = this;
			controlState = "painting grouped window extension";
			if (line)
			{
				//Seperator instead of border
				Rectangle newRect = ClientRectangle;
				newRect.X += Width - 22;
				newRect.Width = 2;
				newRect.Y += 7;
				newRect.Height -= 12;
				ControlPaint.DrawBorder3D(e.Graphics, newRect, Border3DStyle.Raised);
			}
			else
			{
				//Border
				Rectangle newRect = ClientRectangle;
				newRect.X += Width - 19;
				newRect.Width -= Width - 19 + 3;
				newRect.Y += 7;
				newRect.Height -= 12;
				RECT rect = new RECT(newRect);
				uint buttonStyle = !GroupWindow.Visible ? DFCS_BUTTONPUSH : DFCS_BUTTONPUSH | DFCS_PUSHED;
				DrawFrameControl(e.Graphics.GetHdc(), ref rect, DFC_BUTTON, buttonStyle);
				e.Graphics.ReleaseHdc();
				e.Graphics.ResetTransform();
			}

			StringFormat format = new StringFormat();
			format.HotkeyPrefix = HotkeyPrefix.None;
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			format.Trimming = StringTrimming.EllipsisCharacter;
			e.Graphics.DrawString(ProgramWindows.Count.ToString(), new Font(GetFont, FontStyle.Regular), SystemBrushes.ControlText, GroupWindow.Visible ? new Rectangle(Width - 15, 11, 7, 10) : new Rectangle(Width - 16, 10, 7, 11), format);
		}

		public override void OnClick(object sender, MouseEventArgs e)
		{
			if (e.X > Width - 19 &&
				e.X < Width - 3 &&
				e.Y > 7 &&
				e.Y < 24)
			{
				GroupWindow.Show(PointToScreen(new Point(0, 0)));
			}
			else if (IsMoving)
			{
				IsMoving = false;
			}
			else
			{
				ProgramWindows[0].OnClick(sender, e);
				ActiveWindow = ProgramWindows[0].ActiveWindow;
			}
		}

		protected override bool CancelMouseDown(MouseEventArgs e)
		{
			if (e.X > Width - 19 &&
				e.X < Width - 3 &&
				e.Y > 7 &&
				e.Y < 24)
			{
				return true;
			}
			return false;
		}

		public bool ContainsWindow(IntPtr hwnd)
		{
			foreach (SingleTaskbarProgram program in ProgramWindows)
				if (program.Window.Handle == hwnd)
					return true;
			return false;
		}

		public bool UpdateWindowList(List<Window> windows)
		{
			//The new list of icons
			List<SingleTaskbarProgram> newIcons = new List<SingleTaskbarProgram>();

			//Create a new list with only the windows that are still open
			foreach (SingleTaskbarProgram baseIcon in ProgramWindows)
			{
				if (baseIcon is SingleTaskbarProgram singleIcon)
				{
					bool contains = false;
					foreach (Window z in windows)
						if (z.Handle == singleIcon.Window.Handle)
						{
							contains = true;
							singleIcon.Window = z;
						}

					if (contains)
						newIcons.Add(singleIcon);
				}
			}

			//Remove controls of the windows that were removed from the list
			SingleTaskbarProgram[] enumerator = new SingleTaskbarProgram[ProgramWindows.Count];
			ProgramWindows.CopyTo(enumerator);
			foreach (SingleTaskbarProgram dd in enumerator)
			{
				if (dd is SingleTaskbarProgram)
				{
					SingleTaskbarProgram icon = dd as SingleTaskbarProgram;
					if (!newIcons.Contains(icon))
					{
						ProgramWindows.Remove(icon);
						icon.Dispose();
					}
				}
			}

			return ProgramWindows.Count > 0;
		}

		public override string ToString()
		{
			if (Config.ProgramGroupCheck == ProgramGroupCheck.Process)
			{
				return $"Process - ID: {Process.Id}, Name: {Process.ProcessName}";
			}
			else if (Config.ProgramGroupCheck == ProgramGroupCheck.FileNameAndPath)
			{
				return $"Filepath - {GetShortPath(Process.MainModule.FileName)}";
			}
			else if (Config.ProgramGroupCheck == ProgramGroupCheck.ModuleName)
			{
				return $"Filename - {Process.MainModule.ModuleName}";
			}
			return "None - I shouldn't be here!";
		}
	}
}
