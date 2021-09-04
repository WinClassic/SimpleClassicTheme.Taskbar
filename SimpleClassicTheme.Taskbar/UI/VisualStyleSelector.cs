using Craftplacer.Windows.VisualStyles;

using SimpleClassicTheme.Taskbar.ThemeEngine.Renderers;
using SimpleClassicTheme.Taskbar.UIElements.StartButton;

using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleClassicTheme.Taskbar.UI
{
    public class VisualStyleSelector : ListBox
    {
        private const int _padding = 8;
        private StartButton _startButton = new StartButton();
        private Taskbar _taskbar = new Taskbar(true) { Dummy = true };

        public VisualStyleSelector()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
        }

        public override DrawMode DrawMode => DrawMode.OwnerDrawVariable;

        public Bitmap[] Previews { get; private set; } = null;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || Items.Count <= 0)
            {
                return;
            }

            e.DrawBackground();

            ColorScheme colorScheme = Items[e.Index] as ColorScheme;

            int textHeight = 0;
            using (var foreColor = new SolidBrush(e.ForeColor))
            {
                var label = GetLabel(colorScheme);

                e.Graphics.DrawString(label, e.Font, foreColor, e.Bounds.X + _padding, e.Bounds.Y + _padding);
                textHeight = (int)e.Graphics.MeasureString(label, e.Font).Height;
            }

            Bitmap preview = GetPreview(e.Index);
            e.Graphics.DrawImage(preview, e.Bounds.X + _padding, e.Bounds.Y + (_padding * 2) + textHeight);

            e.DrawFocusRectangle();
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);

            if (e.Index < 0 || Items.Count <= 0)
            {
                return;
            }

            Bitmap preview = GetPreview(e.Index);

            ColorScheme colorScheme = Items[e.Index] as ColorScheme;
            string label = GetLabel(colorScheme);
            int textHeight = (int)e.Graphics.MeasureString(label, Font).Height;

            e.ItemHeight = (_padding * 3) + preview.Height + textHeight;
        }

        private static string GetLabel(ColorScheme colorScheme)
        {
            VisualStyle visualStyle = colorScheme.VisualStyle;

            var colorDisplayName = visualStyle.GetColorDisplay(colorScheme.ColorName).DisplayName;
            var label = visualStyle.DisplayName;

            if (colorDisplayName != label)
            {
                label += $" ({colorDisplayName})";
            }

            label += $" by {visualStyle.Author}";

            return label;
        }

        private void GeneratePreviews()
        {
            IList items = Items;
            Previews = new Bitmap[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                ColorScheme colorScheme = items[i] as ColorScheme;
                Previews[i] = GetPreview(colorScheme);
            }
        }

        private Bitmap GetPreview(int index)
        {
            if (Previews == null)
            {
                GeneratePreviews();
            }

            return Previews[index];
        }

        private Bitmap GetPreview(ColorScheme colorScheme)
        {
            VisualStyleRenderer renderer = new VisualStyleRenderer(colorScheme);
            Bitmap previewBitmap = new Bitmap(Width - (_padding * 2), renderer.TaskbarHeight);

            using (Graphics previewGraphics = Graphics.FromImage(previewBitmap))
            {
                PaintPreview(renderer, previewGraphics);
            }

            return previewBitmap;
        }

        private void PaintPreview(BaseRenderer renderer, Graphics g)
        {
            _taskbar.Height = renderer.TaskbarHeight;
            renderer.DrawTaskBar(_taskbar, g);

            _startButton.Height = renderer.TaskbarHeight;
            renderer.DrawStartButton(_startButton, g);

            g.TranslateTransform(renderer.StartButtonWidth, 0);
        }
    }
}