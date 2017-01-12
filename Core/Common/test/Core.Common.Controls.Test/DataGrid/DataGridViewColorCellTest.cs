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
using System.Drawing.Imaging;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewColorCellTest
    {
        [Test]
        public void DefaultConstructor_CreatesNewInstance()
        {
            // Call
            var cell = new DataGridViewColorCell();

            // Assert
            Assert.IsInstanceOf<DataGridViewTextBoxCell>(cell);
        }

        [Test]
        public void GivenGridWithColorCell_WhenGridCellIsMadeVisibleAndUnselected_ThenOutlinedSquareOfColorDrawnAsExpected()
        {
            // Given
            var expectedColor = Color.FromKnownColor(new Random(21).NextEnumValue<KnownColor>());
            var view = new DataGridView
            {
                DataSource = new[]
                {
                    new ColorRow(expectedColor)
                }
            };
            var cell = new DataGridViewColorCell();
            view.Columns.Add(new DataGridViewColorColumn
                             {
                                 CellTemplate = cell,
                                 DataPropertyName = nameof(ColorRow.Color)
                             });

            // When
            WindowsFormsTestHelper.ShowModal(view, f =>
            {
                view.ClearSelection();

                // Then
                var cellDisplayRectangle = view.GetCellDisplayRectangle(0, 0, false);

                using (Bitmap viewDrawCanvas = new Bitmap(view.Width, view.Height))
                using (Image expectedImage = new Bitmap(cellDisplayRectangle.Width, cellDisplayRectangle.Height))
                {
                    view.DrawToBitmap(viewDrawCanvas, new Rectangle(0, 0, view.Width, view.Height));
                    using (var actualImage = viewDrawCanvas.Clone(cellDisplayRectangle, viewDrawCanvas.PixelFormat))
                    {
                        var expectedWidth = cellDisplayRectangle.Width;
                        var expectedHeight = cellDisplayRectangle.Height;
                        var colorRectangle = new Rectangle(3, 3, expectedWidth - 8, expectedHeight - 8);

                        var expectedGraphic = Graphics.FromImage(expectedImage);
                        expectedGraphic.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, expectedWidth, expectedHeight));
                        expectedGraphic.FillRectangle(new SolidBrush(expectedColor), colorRectangle);
                        expectedGraphic.DrawRectangle(new Pen(Color.DarkSlateGray), colorRectangle);
                        expectedGraphic.DrawLine(new Pen(view.GridColor), expectedWidth - 1, 0, expectedWidth - 1, expectedHeight - 1);
                        expectedGraphic.DrawLine(new Pen(view.GridColor), 0, expectedHeight - 1, expectedWidth - 1, expectedHeight - 1);

                        TestHelper.AssertImagesAreEqual(expectedImage, actualImage);
                    }
                }
            });
        }
    }

    public class ColorRow 
    {
        public ColorRow(Color color)
        {
            Color = color;
        }

        public Color Color { get; }
    }
}