// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.TreeView
{
    public static class TreeNodeGraphicExtensions
    {
        private const int defaultImageWidth = 16;
        private const int spaceBetweenNodeParts = 2;

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

            graphics.DrawLine(new Pen(Color.Black, 1), new Point(GetImageLeft(node), yLine), new Point(node.Bounds.Right, yLine));
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
                    xPos = GetImageLeft(node) - placeHolderWidth;
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

        private static int GetImageLeft(TreeNode node)
        {
            return node.Bounds.Left - (defaultImageWidth + spaceBetweenNodeParts);
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