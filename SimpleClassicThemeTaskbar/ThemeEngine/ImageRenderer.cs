using SimpleClassicThemeTaskbar.Helpers;
using SimpleClassicThemeTaskbar.ThemeEngine;
using SimpleClassicThemeTaskbar.UIElements.QuickLaunch;
using SimpleClassicThemeTaskbar.UIElements.StartButton;
using SimpleClassicThemeTaskbar.UIElements.SystemTray;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using static SimpleClassicThemeTaskbar.Helpers.NativeMethods.WinDef;

namespace SimpleClassicThemeTaskbar.ThemeEngine
{
    class ImageRenderer : BaseRenderer
	{
        readonly ClassicRenderer r = new();

		private readonly Bitmap taskbarTexture;
		private readonly Bitmap taskButtonNormal;
		private readonly Bitmap taskButtonNormalHover;
		private readonly Bitmap taskButtonPressed;
		private readonly Bitmap taskButtonPressedHover;
		private readonly (Bitmap, Bitmap, Bitmap)[] taskButtonTexture;
		private readonly Bitmap taskButtonGroupWindowBorder;
		private (Bitmap, Bitmap, Bitmap, Bitmap, Bitmap, Bitmap, Bitmap, Bitmap) taskButtonGroupWindowBorderTexture;
		private readonly Bitmap systemTrayTexture;
		private readonly Bitmap systemTrayBorder;
		private readonly Bitmap startButton;

		private readonly Helpers.IniFile settings;

		private readonly int taskbarHeight;
		private (int, int) taskbuttonTextureLocation;
		private (int, int) taskbuttonLeftBorderLocation;
		private (int, int) taskbuttonRightBorderLocation;
		private readonly int taskbuttonGroupWindowBorderSize;
		private RECT taskbuttonGroupWindowTaskbuttonRealSize;
		private (int, int) taskbuttonIconLocation;
		private (int, int) taskbuttonIconSize;
		private (int, int) taskbuttonTextLocation;
		private readonly int taskbuttonMinimalWidth;
		private readonly int startButtonWidth;
		private readonly int systemTrayBaseWidth;
		private (int, int) systemTrayFirstIconPosition;

		public override int StartButtonWidth => startButtonWidth;
		public override int TaskbarHeight => taskbarHeight;
		public override int TaskButtonMinimalWidth => taskbuttonMinimalWidth;

		public ImageRenderer(string resourceDirectory)
		{
			//try
			if (true)
			{
				settings = new Helpers.IniFile(Path.Combine(resourceDirectory, "Settings.txt"));

				string tH = settings.IniReadValue("TaskBar", "Height");
				if (!Int32.TryParse(tH, out taskbarHeight))
					throw new FileFormatException("[INI] TaskBar/Height must be a valid 32-bit integer");

				Color ttSc = Color.Transparent;
				string ttS = settings.IniReadValue("TaskBar", "TextureSource");
				if (ttS != "File" && !ParseColor(tH, out ttSc))
					throw new FileFormatException("[INI] TaskBar/TextureSource must be either a valid hex color value (with #) or the string 'File'");

				string tmW = settings.IniReadValue("TaskButton", "MinimalWidth");
				if (!Int32.TryParse(tmW, out taskbuttonMinimalWidth))
					throw new FileFormatException("[INI] TaskButton/MinimalWidth must be a valid 32-bit integer");

				string ttL = settings.IniReadValue("TaskButton", "TextureLocation");
				if (!Int32.TryParse(ttL.Split(',')[0], out taskbuttonTextureLocation.Item1) ||
					!Int32.TryParse(ttL.Split(',')[1], out taskbuttonTextureLocation.Item2))
					throw new FileFormatException("[INI] TaskButton/TextureLocation must consist of two valid 32-bit integers seperated by a comma (',')");

				string tlbL = settings.IniReadValue("TaskButton", "LeftBorderLocation");
				if (!Int32.TryParse(tlbL.Split(',')[0], out taskbuttonLeftBorderLocation.Item1) ||
					!Int32.TryParse(tlbL.Split(',')[1], out taskbuttonLeftBorderLocation.Item2))
					throw new FileFormatException("[INI] TaskButton/LeftBorderLocation must consist of two valid 32-bit integers seperated by a comma (',')");

				string trbL = settings.IniReadValue("TaskButton", "RightBorderLocation");
				if (!Int32.TryParse(trbL.Split(',')[0], out taskbuttonRightBorderLocation.Item1) ||
					!Int32.TryParse(trbL.Split(',')[1], out taskbuttonRightBorderLocation.Item2))
					throw new FileFormatException("[INI] TaskButton/RightBorderLocation must consist of two valid 32-bit integers seperated by a comma (',')");

				string tgwbS = settings.IniReadValue("TaskButtonGroupWindow", "BorderSize");
				if (!Int32.TryParse(tgwbS, out taskbuttonGroupWindowBorderSize))
					throw new FileFormatException("[INI] TaskButtonGroupWindow/BorderSize must be a valid 32-bit integer");

				string tgwtrS = settings.IniReadValue("TaskButtonGroupWindow", "TaskButtonRealSize");
				if (!Int32.TryParse(tgwtrS.Split(',')[0], out taskbuttonGroupWindowTaskbuttonRealSize.Left) ||
					!Int32.TryParse(tgwtrS.Split(',')[1], out taskbuttonGroupWindowTaskbuttonRealSize.Top) ||
					!Int32.TryParse(tgwtrS.Split(',')[2], out taskbuttonGroupWindowTaskbuttonRealSize.Right) ||
					!Int32.TryParse(tgwtrS.Split(',')[3], out taskbuttonGroupWindowTaskbuttonRealSize.Bottom))
					throw new FileFormatException("[INI] TaskButtonGroupWindow/TaskButtonRealSize must consist of four valid 32-bit integers seperated by a comma (',')");

				string sbW = settings.IniReadValue("StartButton", "Width");
				if (!Int32.TryParse(sbW, out startButtonWidth))
					throw new FileFormatException("[INI] StartButton/Width must be a valid 32-bit integer");

				string stbW = settings.IniReadValue("SystemTray", "BaseWidth");
				if (!Int32.TryParse(stbW, out systemTrayBaseWidth))
					throw new FileFormatException("[INI] SystemTray/BaseWidth must be a valid 32-bit integer");

				string stfiP = settings.IniReadValue("SystemTray", "FirstIconPosition");
				if (!Int32.TryParse(stfiP.Split(',')[0], out systemTrayFirstIconPosition.Item1) ||
					!Int32.TryParse(stfiP.Split(',')[1], out systemTrayFirstIconPosition.Item2))
					throw new FileFormatException("[INI] SystemTray/FirstIconPosition must consist of two valid 32-bit integers seperated by a comma (',')");

				if (ttS == "File")
					taskbarTexture = new Bitmap(Path.Combine(resourceDirectory, "TaskbarTexture.png"));
				else
					taskbarTexture = GetBitmapFromColor(ttSc);

				taskButtonNormal = new Bitmap(Path.Combine(resourceDirectory, "TaskButtonNormal.png"));
				taskButtonNormalHover = new Bitmap(Path.Combine(resourceDirectory, "TaskButtonNormalHover.png"));
				taskButtonPressed = new Bitmap(Path.Combine(resourceDirectory, "TaskButtonPressed.png"));
				taskButtonPressedHover = new Bitmap(Path.Combine(resourceDirectory, "TaskButtonPressedHover.png"));
				taskButtonTexture = new (Bitmap, Bitmap, Bitmap)[4];
				taskButtonTexture[0] = (taskButtonNormal.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat),
										taskButtonNormal.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat),
										taskButtonNormal.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat));
				taskButtonTexture[1] = (taskButtonNormalHover.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat),
										taskButtonNormalHover.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat),
										taskButtonNormalHover.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat));
				taskButtonTexture[2] = (taskButtonPressed.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat),
										taskButtonPressed.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat),
										taskButtonPressed.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat));
				taskButtonTexture[3] = (taskButtonPressedHover.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat),
										taskButtonPressedHover.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat),
										taskButtonPressedHover.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat));

				taskButtonGroupWindowBorder = new Bitmap(Path.Combine(resourceDirectory, "TaskButtonGroupWindowBorder.png"));
				int cx = taskButtonGroupWindowBorder.Width;
				int cy = taskButtonGroupWindowBorder.Height;
				int bs = taskbuttonGroupWindowBorderSize;
				taskButtonGroupWindowBorderTexture.Item1 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, 0, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Top left corner texture
				taskButtonGroupWindowBorderTexture.Item2 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, 0, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Top right corner texture
				taskButtonGroupWindowBorderTexture.Item3 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, cy - bs, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom left corner texture
				taskButtonGroupWindowBorderTexture.Item4 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, cy - bs, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom right corner texture
				taskButtonGroupWindowBorderTexture.Item5 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, bs, bs, cy - (2 * bs)), taskButtonGroupWindowBorder.PixelFormat); // Left side texture
				taskButtonGroupWindowBorderTexture.Item6 = taskButtonGroupWindowBorder.Clone(new Rectangle(bs, 0, cx - (2 * bs), bs), taskButtonGroupWindowBorder.PixelFormat); // Top side texture
				taskButtonGroupWindowBorderTexture.Item7 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, bs, bs, cy - (2 * bs)), taskButtonGroupWindowBorder.PixelFormat); // Right side texture
				taskButtonGroupWindowBorderTexture.Item8 = taskButtonGroupWindowBorder.Clone(new Rectangle(bs, cy - bs, cx - (2* bs), bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom side texture

				systemTrayTexture = new Bitmap(Path.Combine(resourceDirectory, "SystemTrayTexture.png"));
				systemTrayBorder = new Bitmap(Path.Combine(resourceDirectory, "SystemTrayBorder.png"));
				startButton = new Bitmap(Path.Combine(resourceDirectory, "StartButton.png"));
			}
			//catch { }
		}

		public ImageRenderer(ResourceManager resources)
		{
			string tH = resources.GetString("TaskBar.Height");
			if (!Int32.TryParse(tH, out taskbarHeight))
				throw new FileFormatException("[RESX] TaskBar/Height must be a valid 32-bit integer");

			Color ttSc = Color.Transparent;
			string ttS = resources.GetString("TaskBar.TextureSource");
			if (ttS != "File" && !ParseColor(tH, out ttSc))
				throw new FileFormatException("[RESX] TaskBar/TextureSource must be either a valid hex color value (with #) or the string 'File'");

			string tmW = resources.GetString("TaskButton.MinimalWidth");
			if (!Int32.TryParse(tmW, out taskbuttonMinimalWidth))
				throw new FileFormatException("[RESX] TaskButton/MinimalWidth must be a valid 32-bit integer");

			string ttL = resources.GetString("TaskButton.TextureLocation");
			if (!Int32.TryParse(ttL.Split(',')[0], out taskbuttonTextureLocation.Item1) ||
				!Int32.TryParse(ttL.Split(',')[1], out taskbuttonTextureLocation.Item2))
				throw new FileFormatException("[RESX] TaskButton/TextureLocation must consist of two valid 32-bit integers seperated by a comma (',')");

			string tlbL = resources.GetString("TaskButton.LeftBorderLocation");
			if (!Int32.TryParse(tlbL.Split(',')[0], out taskbuttonLeftBorderLocation.Item1) ||
				!Int32.TryParse(tlbL.Split(',')[1], out taskbuttonLeftBorderLocation.Item2))
				throw new FileFormatException("[RESX] TaskButton/LeftBorderLocation must consist of two valid 32-bit integers seperated by a comma (',')");

			string trbL = resources.GetString("TaskButton.RightBorderLocation");
			if (!Int32.TryParse(trbL.Split(',')[0], out taskbuttonRightBorderLocation.Item1) ||
				!Int32.TryParse(trbL.Split(',')[1], out taskbuttonRightBorderLocation.Item2))
				throw new FileFormatException("[RESX] TaskButton/RightBorderLocation must consist of two valid 32-bit integers seperated by a comma (',')");

			string tgwbS = resources.GetString("TaskButtonGroupWindow.BorderSize");
			if (!Int32.TryParse(tgwbS, out taskbuttonGroupWindowBorderSize))
				throw new FileFormatException("[RESX] TaskButtonGroupWindow/BorderSize must be a valid 32-bit integer");

			string tgwtrS = resources.GetString("TaskButtonGroupWindow.TaskButtonRealSize");
			if (!Int32.TryParse(tgwtrS.Split(',')[0], out taskbuttonGroupWindowTaskbuttonRealSize.Left) ||
				!Int32.TryParse(tgwtrS.Split(',')[1], out taskbuttonGroupWindowTaskbuttonRealSize.Top) ||
				!Int32.TryParse(tgwtrS.Split(',')[2], out taskbuttonGroupWindowTaskbuttonRealSize.Right) ||
				!Int32.TryParse(tgwtrS.Split(',')[3], out taskbuttonGroupWindowTaskbuttonRealSize.Bottom))
				throw new FileFormatException("[RESX] TaskButtonGroupWindow/TaskButtonRealSize must consist of four valid 32-bit integers seperated by a comma (',')");

			string sbW = resources.GetString("StartButton.Width");
			if (!Int32.TryParse(sbW, out startButtonWidth))
				throw new FileFormatException("[RESX] StartButton/Width must be a valid 32-bit integer");

			string stbW = resources.GetString("SystemTray.BaseWidth");
			if (!Int32.TryParse(stbW, out systemTrayBaseWidth))
				throw new FileFormatException("[RESX] SystemTray/BaseWidth must be a valid 32-bit integer");

			string stfiP = resources.GetString("SystemTray.FirstIconPosition");
			if (!Int32.TryParse(stfiP.Split(',')[0], out systemTrayFirstIconPosition.Item1) ||
				!Int32.TryParse(stfiP.Split(',')[1], out systemTrayFirstIconPosition.Item2))
				throw new FileFormatException("[RESX] SystemTray/FirstIconPosition must consist of two valid 32-bit integers seperated by a comma (',')");

			if (ttS == "File")
				taskbarTexture = (Bitmap)resources.GetObject("TaskbarTexture");
			else
				taskbarTexture = GetBitmapFromColor(ttSc);

			taskButtonNormal = (Bitmap)resources.GetObject("TaskButtonNormal");
			taskButtonNormalHover = (Bitmap)resources.GetObject("TaskButtonNormalHover");
			taskButtonPressed = (Bitmap)resources.GetObject("TaskButtonPressed");
			taskButtonPressedHover = (Bitmap)resources.GetObject("TaskButtonPressedHover");
			taskButtonTexture = new (Bitmap, Bitmap, Bitmap)[4];
			taskButtonTexture[0] = (taskButtonNormal.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat),
									taskButtonNormal.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat),
									taskButtonNormal.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonNormal.Height), taskButtonNormal.PixelFormat));
			taskButtonTexture[1] = (taskButtonNormalHover.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat),
									taskButtonNormalHover.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat),
									taskButtonNormalHover.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonNormalHover.Height), taskButtonNormalHover.PixelFormat));
			taskButtonTexture[2] = (taskButtonPressed.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat),
									taskButtonPressed.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat),
									taskButtonPressed.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonPressed.Height), taskButtonPressed.PixelFormat));
			taskButtonTexture[3] = (taskButtonPressedHover.Clone(new Rectangle(taskbuttonTextureLocation.Item1, 0, taskbuttonTextureLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat),
									taskButtonPressedHover.Clone(new Rectangle(taskbuttonLeftBorderLocation.Item1, 0, taskbuttonLeftBorderLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat),
									taskButtonPressedHover.Clone(new Rectangle(taskbuttonRightBorderLocation.Item1, 0, taskbuttonRightBorderLocation.Item2, taskButtonPressedHover.Height), taskButtonPressedHover.PixelFormat));

			taskButtonGroupWindowBorder = (Bitmap)resources.GetObject("TaskButtonGroupWindowBorder");
			int cx = taskButtonGroupWindowBorder.Width;
			int cy = taskButtonGroupWindowBorder.Height;
			int bs = taskbuttonGroupWindowBorderSize;
			taskButtonGroupWindowBorderTexture.Item1 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, 0, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Top left corner texture
			taskButtonGroupWindowBorderTexture.Item2 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, 0, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Top right corner texture
			taskButtonGroupWindowBorderTexture.Item3 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, cy - bs, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom left corner texture
			taskButtonGroupWindowBorderTexture.Item4 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, cy - bs, bs, bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom right corner texture
			taskButtonGroupWindowBorderTexture.Item5 = taskButtonGroupWindowBorder.Clone(new Rectangle(0, bs, bs, cy - (2 * bs)), taskButtonGroupWindowBorder.PixelFormat); // Left side texture
			taskButtonGroupWindowBorderTexture.Item6 = taskButtonGroupWindowBorder.Clone(new Rectangle(bs, 0, cx - (2 * bs), bs), taskButtonGroupWindowBorder.PixelFormat); // Top side texture
			taskButtonGroupWindowBorderTexture.Item7 = taskButtonGroupWindowBorder.Clone(new Rectangle(cx - bs, bs, bs, cy - (2 * bs)), taskButtonGroupWindowBorder.PixelFormat); // Right side texture
			taskButtonGroupWindowBorderTexture.Item8 = taskButtonGroupWindowBorder.Clone(new Rectangle(bs, cy - bs, cx - (2 * bs), bs), taskButtonGroupWindowBorder.PixelFormat); // Bottom side texture

			systemTrayTexture = (Bitmap)resources.GetObject("SystemTrayTexture");
			systemTrayBorder = (Bitmap)resources.GetObject("SystemTrayBorder");
			startButton = (Bitmap)resources.GetObject("StartButton");
		}

		public override Size GetTaskButtonGroupWindowSize(int buttonCount) => new Size(Config.Instance.TaskbarProgramWidth + (taskbuttonGroupWindowBorderSize * 2), ((buttonCount - 1) * (TaskbarHeight - taskbuttonGroupWindowTaskbuttonRealSize.Top + taskbuttonGroupWindowTaskbuttonRealSize.Bottom)) + (taskbuttonGroupWindowBorderSize * 2));
		public override Point GetTaskButtonGroupWindowButtonLocation(int index) => new Point(taskbuttonGroupWindowBorderSize, ((index - 1) * (TaskbarHeight - taskbuttonGroupWindowTaskbuttonRealSize.Top + taskbuttonGroupWindowTaskbuttonRealSize.Bottom)) - taskbuttonGroupWindowTaskbuttonRealSize.Top + taskbuttonGroupWindowBorderSize);

		public override Point GetSystemTrayIconLocation(int index) => new(systemTrayFirstIconPosition.Item1 + (index * (16 + Config.Instance.SpaceBetweenTrayIcons)), systemTrayFirstIconPosition.Item2);
		public override int GetSystemTrayWidth(int iconCount) => systemTrayBaseWidth + (iconCount * 16) + (Config.Instance.SpaceBetweenTrayIcons * (iconCount - 1));
		public override Point SystemTrayTimeLocation => r.SystemTrayTimeLocation;
		public override Font SystemTrayTimeFont => r.SystemTrayTimeFont;
		public override Color SystemTrayTimeColor => Color.White;

		public override Point GetQuickLaunchIconLocation(int index)
		{
			return r.GetQuickLaunchIconLocation(index);
		}
		public override int GetQuickLaunchWidth(int iconCount)
		{
			return r.GetQuickLaunchWidth(iconCount);
		}

		public override void DrawTaskBar(Taskbar taskbar, Graphics g)
		{
			using (TextureBrush brush = new(taskbarTexture, WrapMode.Tile))
				g.FillRectangle(brush, taskbar.ClientRectangle);
		}

		public override void DrawTaskButton(BaseTaskbarProgram taskbarProgram, Graphics g)
		{
			bool IsActive = taskbarProgram.IsPushed;
			bool IsHover = taskbarProgram.ClientRectangle.Contains(taskbarProgram.PointToClient(Control.MousePosition));
			int index = (IsHover ? 1 : 0) + (IsActive ? 2 : 0);

			// Draw texture
			Bitmap tbTexture = taskButtonTexture[index].Item1;
			using (TextureBrush brush = new(tbTexture, WrapMode.Tile))
				g.FillRectangle(brush, taskbarProgram.ClientRectangle);

			// Draw left border
			Bitmap tblbTexture = taskButtonTexture[index].Item2;
			g.DrawImageUnscaled(tblbTexture, new Point(0, 0));

			// Draw right border
			Bitmap tbrbTexture = taskButtonTexture[index].Item3;
			g.DrawImageUnscaled(tbrbTexture, new Point(taskbarProgram.Width - tbrbTexture.Width, 0));

			// Generate font
			Font font = SystemFonts.DefaultFont;
			if (IsActive)
				font = new Font(font, FontStyle.Bold);

			// Draw text and icon
			StringFormat format = new();
			format.HotkeyPrefix = HotkeyPrefix.None;
			format.Alignment = StringAlignment.Near;
			format.LineAlignment = StringAlignment.Center;
			format.Trimming = StringTrimming.EllipsisCharacter;
			if (taskbarProgram.IconImage != null)
			{
				g.DrawImage(taskbarProgram.IconImage, IsActive ? new Rectangle(5, 8, 16, 16) : new Rectangle(4, 7, 16, 16));

				if (taskbarProgram.Width >= 60)
					g.DrawString(taskbarProgram.Title, font, Brushes.White, IsActive ? new Rectangle(21, 10, taskbarProgram.Width - 21 - 3 - taskbarProgram.SpaceNeededNextToText, 13) : new Rectangle(20, 9, taskbarProgram.Width - 20 - 3 - taskbarProgram.SpaceNeededNextToText, 14), format);
			}
			else
			{
				g.DrawString(taskbarProgram.Title, font, Brushes.White, IsActive ? new Rectangle(5, 10, taskbarProgram.Width - 5 - 3 - taskbarProgram.SpaceNeededNextToText, 13) : new Rectangle(4, 9, taskbarProgram.Width - 4 - 3 - taskbarProgram.SpaceNeededNextToText, 14), format);
			}
		}

		public override void DrawTaskButtonGroupButton(GroupedTaskbarProgram taskbarProgram, Graphics g)
		{
			r.DrawTaskButtonGroupButton(taskbarProgram, g);
		}

		public override void DrawTaskButtonGroupWindow(PopupTaskbarGroup taskbarGroup, Graphics g)
		{
			// Drawing constants
			int bs = taskbuttonGroupWindowBorderSize;
			int cxm = taskbarGroup.Width - bs;
			int cym = taskbarGroup.Height - bs;
			int cxb = taskbarGroup.Width - (2 * bs);
			int cyb = taskbarGroup.Height - (2 * bs);

			// Draw corners
			g.DrawImageUnscaled(taskButtonGroupWindowBorderTexture.Item1, new Point(0, 0));
			g.DrawImageUnscaled(taskButtonGroupWindowBorderTexture.Item2, new Point(cxm, 0));
			g.DrawImageUnscaled(taskButtonGroupWindowBorderTexture.Item3, new Point(0, cym));
			g.DrawImageUnscaled(taskButtonGroupWindowBorderTexture.Item4, new Point(cxm, cym));

			// Draw sides
			using (TextureBrush brush = new(taskButtonGroupWindowBorderTexture.Item5, WrapMode.Tile))
			{
				brush.TranslateTransform(0, bs);
				g.FillRectangle(brush, new Rectangle(0, bs, bs, cyb));
			}
			using (TextureBrush brush = new(taskButtonGroupWindowBorderTexture.Item6, WrapMode.Tile))
			{
				brush.TranslateTransform(bs, 0);
				g.FillRectangle(brush, new Rectangle(bs, 0, cxb, bs));
			}
			using (TextureBrush brush = new(taskButtonGroupWindowBorderTexture.Item7, WrapMode.Tile))
			{
				brush.TranslateTransform(cxm, bs);
				g.FillRectangle(brush, new Rectangle(cxm, bs, bs, cyb));
			}
			using (TextureBrush brush = new(taskButtonGroupWindowBorderTexture.Item8, WrapMode.Tile))
			{
				brush.TranslateTransform(bs, cym);
				g.FillRectangle(brush, new Rectangle(bs, cym, cxb, bs));
			}
				

			//r.DrawTaskButtonGroupWindow(taskbarGroup, g);
		}

		public override void DrawStartButton(StartButton startButton, Graphics g)
		{
			Image image = this.startButton;
			if (startButton.Width != image.Width)
				startButton.Width = image.Width;
			g.DrawImage(image, new Rectangle(0, 0, image.Width, taskbarHeight), new Rectangle(0, startButton.Pressed ? (taskbarHeight*2) : startButton.ClientRectangle.Contains(startButton.PointToClient(Control.MousePosition)) ? taskbarHeight : 0, image.Width, taskbarHeight), GraphicsUnit.Pixel);
		}

		public override void DrawSystemTray(SystemTray systemTray, Graphics g)
		{
			using (TextureBrush brush = new(systemTrayTexture, WrapMode.Tile))
				g.FillRectangle(brush, systemTray.ClientRectangle);
			g.DrawImageUnscaled(systemTrayBorder, new Point(0, 0));

			//r.DrawSystemTray(systemTray, g);
		}

		public override void DrawQuickLaunch(QuickLaunch quickLaunch, Graphics g)
		{
			r.DrawQuickLaunch(quickLaunch, g);
		}

		private static bool ParseColor(string text, out Color c)
		{
			Match match = Regex.Match(text, "#[a-f0-9A-F]{6}");
			if (match != null)
			{
				string color = match.Value;
				c = Color.FromArgb(Int32.Parse(color.Substring(1, 2)),
					Int32.Parse(color.Substring(3, 2)),
					Int32.Parse(color.Substring(5, 2)));
				return true;
			}
			c = Color.Transparent;
			return false;
		}

		private static Bitmap GetBitmapFromColor(Color c)
		{
			Bitmap b = new(1, 1);
			b.SetPixel(0, 0, c);
			return b;
		}
	}
}
