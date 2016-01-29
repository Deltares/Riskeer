﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Drawing;
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

        private static void DrawImage(Graphics graphics, TreeNode node, TreeNodeInfo treeNodeInfo)
        {
            var image = treeNodeInfo.Image(node.Tag);
            var graphicsUnit = GraphicsUnit.Pixel;
            var topOffset = (node.Bounds.Height - defaultImageHeight)/2;
            var imgRect = new Rectangle(GetImageLeft(node, treeNodeInfo), node.Bounds.Top + topOffset, defaultImageWidth, defaultImageHeight);

            graphics.DrawImage(image, imgRect, image.GetBounds(ref graphicsUnit), graphicsUnit);
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