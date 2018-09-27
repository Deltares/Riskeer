// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// This class define a cell for a data grid view in which a color is shown based
    /// on the value of that cell.
    /// </summary>
    public class DataGridViewColorCell : DataGridViewTextBoxCell
    {
        protected override void Paint(
            Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            base.Paint(
                graphics,
                clipBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                string.Empty,
                errorText,
                cellStyle,
                advancedBorderStyle,
                paintParts);

            if (paintParts.HasFlag(DataGridViewPaintParts.ContentBackground) && value is Color)
            {
                using (var cellBackground = new SolidBrush((Color) value))
                using (var cellBackgroundBorder = new Pen(Color.DarkSlateGray, 1))
                {
                    Rectangle rectangleWithMargin = CreateRectangleWithMargin(cellBounds, 3);
                    graphics.FillRectangle(cellBackground, rectangleWithMargin);
                    graphics.DrawRectangle(cellBackgroundBorder, rectangleWithMargin);
                }
            }
        }

        private static Rectangle CreateRectangleWithMargin(Rectangle cellBounds, int i)
        {
            return new Rectangle(
                cellBounds.X + i,
                cellBounds.Y + i,
                cellBounds.Width - i * 2 - 2,
                cellBounds.Height - i * 2 - 2);
        }
    }
}