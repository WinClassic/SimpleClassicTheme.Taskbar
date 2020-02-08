using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SimpleClassicThemeTaskbar
{
    public partial class VertSep : Control
    {
        private Color lineColor;
        private Pen linePen;

        public VertSep()
        {
            LineColor = Color.LightGray;
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        public Color LineColor
        {
            get { return this.lineColor; }
            set
            {
                this.lineColor = value;

                this.linePen = new Pen(this.lineColor, 1);
                this.linePen.Alignment = PenAlignment.Inset;

                Refresh();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.linePen != null)
            {
                this.linePen.Dispose();
                this.linePen = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            int x = this.Width / 2;

            g.DrawLine(linePen, x, 0, x, this.Height);

            base.OnPaint(e);
        }
    }
}