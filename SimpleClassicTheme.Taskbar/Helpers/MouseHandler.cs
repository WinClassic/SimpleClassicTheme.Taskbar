using System;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public static class MouseHandler
    {
        public static void SubscribeToMouseEvents(this Control control)
        {
            if (control is not IHasMouseState mouseClass)
            {
                throw new ArgumentException($"Provided control parameter does not implement {nameof(IHasMouseState)}", nameof(control));
            }

            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
        }

        private static void Control_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Control control && sender is IHasMouseState mouseClass)
            {
                mouseClass.IsHovered = false;
                control.Invalidate();
            }
        }

        private static void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && sender is IHasMouseState mouseClass)
            {
                mouseClass.IsHovered = true;
                control.Invalidate();
            }
        }

        private static void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Control control && sender is IHasMouseState mouseClass)
            {
                mouseClass.IsPressed = false;
                control.Invalidate();
            }
        }

        private static void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control control && sender is IHasMouseState mouseClass)
            {
                mouseClass.IsPressed = true;
                control.Invalidate();
            }
        }
    }

    public interface IHasMouseState
    {
        public bool IsPressed { get; set; }

        public bool IsHovered { get; set; }

        public MouseState MouseState
        {
            get
            {
                if (IsPressed)
                {
                    return MouseState.Pressed;
                }
                else if (IsHovered)
                {
                    return MouseState.Hover;
                }
                else
                {
                    return MouseState.Normal;
                }
            }
        }
    }
}
