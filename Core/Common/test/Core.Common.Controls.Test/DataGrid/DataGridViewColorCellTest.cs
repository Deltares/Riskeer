// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Drawing;
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
            using (var cell = new DataGridViewColorCell())
            {
                // Assert
                Assert.IsInstanceOf<DataGridViewTextBoxCell>(cell);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetColors))]
        public void GivenGridWithColorCell_WhenGridCellIsMadeVisibleAndUnselected_ThenOutlinedSquareOfColorDrawnAsExpected(Color expectedColor)
        {
            // Given
            var view = new DataGridView
            {
                DataSource = new[]
                {
                    new ColorRow(expectedColor)
                }
            };
            using (var cell = new DataGridViewColorCell())
            {
                view.Columns.Add(new DataGridViewColorColumn
                {
                    CellTemplate = cell,
                    DataPropertyName = nameof(ColorRow.Color)
                });

                // When
                WindowsFormsTestHelper.ShowModal(
                    view,
                    f =>
                    {
                        view.ClearSelection();

                        // Then
                        Rectangle cellDisplayRectangle = view.GetCellDisplayRectangle(0, 0, false);

                        using (var viewDrawCanvas = new Bitmap(view.Width, view.Height))
                        using (Bitmap expectedImage = CreateExpectedImage(cellDisplayRectangle, expectedColor, view.GridColor))
                        {
                            view.DrawToBitmap(viewDrawCanvas, new Rectangle(0, 0, view.Width, view.Height));
                            using (Bitmap actualImage = viewDrawCanvas.Clone(cellDisplayRectangle, viewDrawCanvas.PixelFormat))
                            {
                                TestHelper.AssertImagesAreEqual(expectedImage, actualImage);
                            }
                        }
                    });
            }
        }

        private static IEnumerable<TestCaseData> GetColors()
        {
            yield return new TestCaseData(Color.White);
            yield return new TestCaseData(Color.HotPink);
            yield return new TestCaseData(Color.Aqua);
            yield return new TestCaseData(Color.Empty);
            yield return new TestCaseData(Color.Transparent);
        }

        private static Bitmap CreateExpectedImage(Rectangle cellDisplayRectangle, Color cellValueColor, Color borderColor)
        {
            int width = cellDisplayRectangle.Width;
            int height = cellDisplayRectangle.Height;
            var expectedImage = new Bitmap(width, height);
            var colorRectangle = new Rectangle(3, 3, width - 8, height - 8);

            Graphics expectedGraphic = Graphics.FromImage(expectedImage);
            expectedGraphic.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            expectedGraphic.FillRectangle(new SolidBrush(cellValueColor), colorRectangle);
            expectedGraphic.DrawRectangle(new Pen(Color.DarkSlateGray), colorRectangle);
            expectedGraphic.DrawLine(new Pen(borderColor), width - 1, 0, width - 1, height - 1);
            expectedGraphic.DrawLine(new Pen(borderColor), 0, height - 1, width - 1, height - 1);

            return expectedImage;
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