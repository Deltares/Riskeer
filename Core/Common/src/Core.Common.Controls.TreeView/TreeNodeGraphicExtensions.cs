using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Core.Common.Controls.TreeView
{
    public static class TreeNodeGraphicExtensions
    {
        private const int defaultImageWidth = 16;
        private const int defaultImageHeight = 16;
        private const int spaceBetweenNodeParts = 2;

        /// <summary>
        /// Checks if the <paramref name="point"/> is on the checkbox of the node
        /// </summary>
        /// <param name="node">Node to check for</param>
        /// <param name="point">Point to search for</param>
        public static bool IsOnCheckBox(this TreeNode node, Point point)
        {
            var topOffset = (node.Bounds.Height - defaultImageHeight)/2;
            var rectangle = new Rectangle(GetCheckBoxLeft(node), node.Bounds.Top + topOffset, defaultImageWidth, defaultImageHeight);
            return rectangle.Contains(point);
        }

        /// <summary>
        /// Draws a tree node using the boundaries of the node
        /// </summary>
        /// <param name="node">Node to draw</param>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo"/> to use while drawing the node.</param>
        /// <param name="graphics">Graphic to draw on</param>
        /// <param name="selected">Is node in selected state</param>
        public static void DrawNode(this TreeNode node, TreeNodeInfo treeNodeInfo, Graphics graphics, bool selected)
        {
            if (node.Bounds.Height == 0) // Nothing to draw
            {
                return;
            }

            DrawText(graphics, node, treeNodeInfo, selected);
            DrawTreeLines(graphics, node);

            if (treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(node.Tag))
            {
                DrawCheckbox(graphics, node);
            }

            if (treeNodeInfo.Image != null)
            {
                DrawImage(graphics, node, treeNodeInfo);
            }
        }

        /// <summary>
        /// Draws a placeholder for the node on the indicated location
        /// </summary>
        /// <param name="node">Node to draw the placeholder for</param>
        /// <param name="location">Location of the placeholder</param>
        /// <param name="graphics">Graphics to draw on</param>
        public static void DrawPlaceHolder(this TreeNode node, PlaceholderLocation location, Graphics graphics)
        {
            var rightTriangle = node.MakePlaceHoldeTriangle(AnchorStyles.Right, location);

            graphics.FillPolygon(Brushes.Black, rightTriangle);

            if (location == PlaceholderLocation.Middle)
            {
                return;
            }

            var leftTriangle = node.MakePlaceHoldeTriangle(AnchorStyles.Left, location);
            graphics.FillPolygon(Brushes.Black, leftTriangle);

            var yLine = location == PlaceholderLocation.Top
                            ? node.Bounds.Top
                            : node.Bounds.Bottom;

            graphics.DrawLine(new Pen(Color.Black, 1), new Point(GetCheckBoxLeft(node), yLine), new Point(node.Bounds.Right, yLine));
        }

        private static Point[] MakePlaceHoldeTriangle(this TreeNode node, AnchorStyles anchor, PlaceholderLocation location)
        {
            const int placeHolderWidth = 4;
            const int placeHolderHeigth = 8;

            int xPos, yPos;
            var bounds = node.Bounds;

            switch (anchor)
            {
                case AnchorStyles.Left:
                    xPos = GetCheckBoxLeft(node) - placeHolderWidth;
                    break;
                case AnchorStyles.Right:
                    xPos = bounds.Right;
                    break;
                default:
                    return new Point[0];
            }

            switch (location)
            {
                case PlaceholderLocation.Top:
                    yPos = bounds.Top;
                    break;
                case PlaceholderLocation.Bottom:
                    yPos = bounds.Bottom;
                    break;
                case PlaceholderLocation.Middle:
                    yPos = bounds.Top + bounds.Height/2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }

            return CreateTrianglePoints(new Rectangle(xPos, yPos - placeHolderWidth, placeHolderWidth, placeHolderHeigth), anchor);
        }

        private static void DrawTreeLines(Graphics graphics, TreeNode node)
        {
            var bounds = node.Bounds;
            var pen = new Pen(Color.Black)
            {
                DashStyle = DashStyle.Dot
            };
            var hasNextNodeOnSameLevel = GetNextNodeSameLevel(node) != null;
            var yBoundsMiddle = bounds.Top + bounds.Height/2;
            var xLine = GetTreeLineLeft(node);

            if (node.Parent != null)
            {
                graphics.DrawLine(pen, xLine, bounds.Top, xLine, hasNextNodeOnSameLevel ? bounds.Bottom : yBoundsMiddle); // Vertical line
            }
            if (node.Parent != null || node.Nodes.Count > 0)
            {
                graphics.DrawLine(pen, xLine + 1, yBoundsMiddle, GetCheckBoxLeft(node) - spaceBetweenNodeParts, yBoundsMiddle); // Horizontal line
            }

            // draw parent lines
            var parentNode = node;
            for (int i = node.Level - 1; i > 0; i--)
            {
                parentNode = parentNode.Parent;
                var nextNodeSameLevel = GetNextNodeSameLevel(parentNode);
                if (nextNodeSameLevel == null)
                {
                    continue;
                }

                var xParentLine = GetTreeLineLeft(parentNode);
                graphics.DrawLine(pen, xParentLine, bounds.Top, xParentLine, bounds.Bottom); // Vertical line parent
            }

            DrawExpandGraphic(graphics, node, yBoundsMiddle);
        }

        private static void DrawExpandGraphic(Graphics graphics, TreeNode node, int yBoundsMiddle)
        {
            if (node.Nodes.Count == 0)
            {
                return;
            }

            if (Application.RenderWithVisualStyles)
            {
                var image = node.IsExpanded
                                ? VisualStyleElement.TreeView.Glyph.Opened
                                : VisualStyleElement.TreeView.Glyph.Closed;

                var renderer = new VisualStyleRenderer(image);
                var buttonSize = GetExpandButtonSize(graphics);
                var drawingRect = new Rectangle(GetTreeLineLeft(node) - buttonSize/2, yBoundsMiddle - buttonSize/2, buttonSize, buttonSize);
                renderer.DrawBackground(graphics, drawingRect);
            }
            else
            {
                const int height = 8;
                const int width = 8;
                var x = GetTreeLineLeft(node) - 4;
                var y = yBoundsMiddle - 4;

                var penBlack = new Pen(new SolidBrush(Color.Black));

                graphics.DrawRectangle(new Pen(SystemBrushes.ControlDark), x, y, width, height);
                graphics.FillRectangle(new SolidBrush(Color.White), x + 1, y + 1, width - 1, height - 1);
                graphics.DrawLine(penBlack, x + 2, y + 4, x + width - 2, y + 4);

                if (node.IsExpanded)
                {
                    return;
                }

                graphics.DrawLine(penBlack, x + 4, y + 2, x + 4, y + height - 2);
            }
        }

        private static void DrawText(Graphics graphics, TreeNode node, TreeNodeInfo treeNodeInfo, bool selected)
        {
            if (node.IsEditing && node.IsSelected)
            {
                return;
            }

            var bounds = node.Bounds;
            var treeView = node.TreeView;
            var foreColor = selected && treeView.Focused
                                ? SystemColors.HighlightText
                                : node.ForeColor != Color.Empty ? node.ForeColor : treeView.ForeColor;

            var backgroundColor = selected
                                      ? treeView.Focused ? SystemColors.Highlight : Color.FromArgb(255, 232, 232, 232)
                                      : node.BackColor != Color.Empty
                                            ? node.BackColor
                                            : treeView.BackColor;

            var font = new Font(node.NodeFont ?? treeView.Font, FontStyle.Regular);
            var topOffset = (node.Bounds.Height - TextRenderer.MeasureText(node.Text, font).Height)/2;

            var startPoint = new Point(GetTextLeft(node, treeNodeInfo), bounds.Top + topOffset);
            var drawingBounds = treeView.FullRowSelect
                                    ? new Rectangle(0, bounds.Top, treeView.Width, bounds.Height)
                                    : new Rectangle(GetTextLeft(node, treeNodeInfo), bounds.Top, bounds.Width, bounds.Height);

            graphics.FillRectangle(new SolidBrush(backgroundColor), drawingBounds);

            TextRenderer.DrawText(graphics, node.Text, font, startPoint, foreColor, backgroundColor, TextFormatFlags.Default);

            if (selected)
            {
                ControlPaint.DrawFocusRectangle(graphics, drawingBounds, foreColor, SystemColors.Highlight);
            }
        }

        private static void DrawCheckbox(Graphics graphics, TreeNode node)
        {
            var topOffset = (node.Bounds.Height - defaultImageHeight)/2;
            var imgRect = new Rectangle(GetCheckBoxLeft(node), node.Bounds.Top + topOffset, defaultImageWidth, defaultImageHeight);

            if (Application.RenderWithVisualStyles)
            {
                var point = new Point(imgRect.Left + spaceBetweenNodeParts, imgRect.Top + 2);
                CheckBoxRenderer.DrawCheckBox(graphics, point,
                                              node.Checked
                                                  ? CheckBoxState.CheckedNormal
                                                  : CheckBoxState.UncheckedNormal);
            }
            else
            {
                ControlPaint.DrawCheckBox(graphics, imgRect,
                                          (node.Checked ? ButtonState.Checked : ButtonState.Normal) |
                                          ButtonState.Flat);
            }
        }

        private static int GetExpandButtonSize(Graphics graphics)
        {
            // 20 at 72 dpi -> convert to local dpi
            return Convert.ToInt32(20*(72/graphics.DpiX));
        }

        private static void DrawImage(Graphics graphics, TreeNode node, TreeNodeInfo treeNodeInfo)
        {
            var image = treeNodeInfo.Image(node.Tag);
            var graphicsUnit = GraphicsUnit.Pixel;
            var topOffset = (node.Bounds.Height - defaultImageHeight)/2;
            var imgRect = new Rectangle(GetImageLeft(node, treeNodeInfo), node.Bounds.Top + topOffset, defaultImageWidth, defaultImageHeight);

            graphics.DrawImage(image, imgRect, image.GetBounds(ref graphicsUnit), graphicsUnit);
        }

        private static int GetTreeLineLeft(TreeNode node)
        {
            return GetCheckBoxLeft(node) - defaultImageWidth + 3;
        }

        private static int GetCheckBoxLeft(TreeNode node)
        {
            return node.Bounds.Left - (defaultImageWidth + spaceBetweenNodeParts);
        }

        private static int GetImageLeft(TreeNode node, TreeNodeInfo treeNodeInfo)
        {
            var xCheckBox = GetCheckBoxLeft(node);

            return treeNodeInfo.CanCheck != null && treeNodeInfo.CanCheck(node.Tag)
                       ? xCheckBox + defaultImageWidth + spaceBetweenNodeParts
                       : xCheckBox;
        }

        private static int GetTextLeft(TreeNode node, TreeNodeInfo treeNodeInfo)
        {
            var xImage = GetImageLeft(node, treeNodeInfo);

            return treeNodeInfo.Image != null && treeNodeInfo.Image(node.Tag) != null
                       ? xImage + defaultImageWidth + spaceBetweenNodeParts
                       : xImage;
        }

        private static TreeNode GetNextNodeSameLevel(TreeNode node)
        {
            var nodes = node.Parent != null ? node.Parent.Nodes : node.TreeView.Nodes;
            var index = nodes.IndexOf(node) + 1;
            return nodes.Count > index ? nodes[index] : null;
        }

        private static Point[] CreateTrianglePoints(Rectangle bounds, AnchorStyles anchor)
        {
            switch (anchor)
            {
                case AnchorStyles.Left:
                    return new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Right, bounds.Top + bounds.Height/2),
                        new Point(bounds.Left, bounds.Top + bounds.Height),
                        new Point(bounds.Left, bounds.Top)
                    };
                case AnchorStyles.Right:
                    return new[]
                    {
                        new Point(bounds.Right, bounds.Top),
                        new Point(bounds.Left, bounds.Top + bounds.Height/2),
                        new Point(bounds.Right, bounds.Top + bounds.Height),
                        new Point(bounds.Right, bounds.Top)
                    };
                default:
                    return new Point[0];
            }
        }
    }
}