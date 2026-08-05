using System.Drawing;
using System.Windows.Forms;

namespace GenieClient.Forms.Controls
{
    /// <summary>
    /// A TabControl that repaints the leftover header-strip area (to the right of the last tab)
    /// and the classic 3D sunken pane border around the page content after the native control
    /// finishes painting. The native comctl32 tab control always fills those areas using hardcoded
    /// classic system colors regardless of the managed BackColor property or owner-draw item
    /// painting, which leaves light gray strips/borders in dark mode. This override paints over
    /// them with the correct dark color after each native paint.
    /// </summary>
    public class CustomTabControl : TabControl
    {
        private const int WM_PAINT = 0x000F;
        private const int WS_EX_CLIENTEDGE = 0x00000200;
        private const int WS_EX_STATICEDGE = 0x00020000;
        private const int WS_BORDER = 0x00800000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= ~WS_EX_CLIENTEDGE;
                cp.ExStyle &= ~WS_EX_STATICEDGE;
                cp.Style &= ~WS_BORDER;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && TabCount > 0)
            {
                PaintHeaderFiller();
                PaintPageBorder();
                PaintTabButtonEdges();
            }
        }

        // The native comctl32 tab control paints its own bevel/edge for each tab button as a
        // final layer on top of everything, including our OwnerDraw DrawItem output - most
        // visible as a light gray edge along the inner border of inactive tabs. Re-paint the
        // full tab button (background + text) one more time, inflated slightly to also cover
        // the native edge just outside the tab rect, so nothing native remains visible.
        private void PaintTabButtonEdges()
        {
            const int edgeThickness = 2;

            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                for (int i = 0; i < TabCount; i++)
                {
                    Rectangle tabRect = GetTabRect(i);
                    Rectangle paintRect = Rectangle.Inflate(tabRect, edgeThickness, edgeThickness);
                    bool isSelected = i == SelectedIndex;
                    Color backColor = isSelected ? SystemColors.ButtonHighlight : SystemColors.ButtonFace;

                    using (var backBrush = new SolidBrush(backColor))
                    {
                        g.FillRectangle(backBrush, paintRect);
                    }

                    TextRenderer.DrawText(g, TabPages[i].Text, Font, tabRect, SystemColors.WindowText,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
        }

        private void PaintHeaderFiller()
        {
            Rectangle lastTabRect = GetTabRect(TabCount - 1);
            int headerHeight = lastTabRect.Bottom + 2;
            var headerRowRect = new Rectangle(0, 0, Width, headerHeight);

            if (headerRowRect.Width <= 0 || headerRowRect.Height <= 0)
            {
                return;
            }

            using (Graphics g = Graphics.FromHwnd(Handle))
            using (var brush = new SolidBrush(SystemColors.Control))
            using (var region = new Region(headerRowRect))
            {
                // Exclude the actual tab button rects so the owner-drawn tab
                // headers (already painted by the native control) aren't erased.
                // What's left is the light gray row background/margins that
                // comctl32 always paints around the tabs regardless of colors.
                for (int i = 0; i < TabCount; i++)
                {
                    region.Exclude(GetTabRect(i));
                }

                g.FillRegion(brush, region);
            }
        }

        private void PaintPageBorder()
        {
            Rectangle lastTabRect = GetTabRect(TabCount - 1);
            int headerHeight = lastTabRect.Bottom + 2;
            const int borderThickness = 4;

            if (Width <= 0 || Height <= headerHeight)
            {
                return;
            }

            using (Graphics g = Graphics.FromHwnd(Handle))
            using (var brush = new SolidBrush(SystemColors.Control))
            {
                // Top edge of the page border (just below the tab header row)
                g.FillRectangle(brush, new Rectangle(0, headerHeight, Width, borderThickness));
                // Bottom edge
                g.FillRectangle(brush, new Rectangle(0, Height - borderThickness, Width, borderThickness));
                // Left edge
                g.FillRectangle(brush, new Rectangle(0, headerHeight, borderThickness, Height - headerHeight));
                // Right edge
                g.FillRectangle(brush, new Rectangle(Width - borderThickness, headerHeight, borderThickness, Height - headerHeight));
            }
        }
    }
}
