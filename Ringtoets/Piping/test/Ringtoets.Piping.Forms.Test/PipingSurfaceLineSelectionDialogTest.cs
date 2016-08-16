// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test
{
    [TestFixture]
    public class PipingSurfaceLineSelectionDialogTest
    {
        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLineSelectionDialog(null, Enumerable.Empty<RingtoetsPipingSurfaceLine>());

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        } 

        [Test]
        public void Constructor_WithoutSurfaceLines_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLineSelectionDialog(new Form(), null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLines", parameter);
        }

        [Test]
        public void Constructor_WithParentAndSurfaceLines_DefaultProperties()
        {
            // Call
            using (var dialog = new PipingSurfaceLineSelectionDialog(new Form(), Enumerable.Empty<RingtoetsPipingSurfaceLine>()))
            {

                // Assert
                Assert.IsEmpty(dialog.SelectedSurfaceLines);
                Assert.IsInstanceOf<PipingSurfaceLineSelectionView>(new ControlTester("PipingSurfaceLineSelectionView", dialog).TheObject);
                Assert.AreEqual("Selecteer profielschematisaties", dialog.Text);
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var dialog = new PipingSurfaceLineSelectionDialog(new Form(), Enumerable.Empty<RingtoetsPipingSurfaceLine>()))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(300, dialog.MinimumSize.Width);
                Assert.AreEqual(400, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void GivenDialogWithSelectedSurfaceLines_WhenCloseWithoutConfirmation_ThenReturnsEmptyCollection()
        {
            // Given
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines))
            {

                var selectionView = (DataGridView) new ControlTester("SurfaceLineDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                dialog.Close();

                // Then
                Assert.IsEmpty(dialog.SelectedSurfaceLines);
            }
        }

        [Test]
        public void GivenDialogWithSelectedSurfaceLines_WhenCancelButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedSurfaceLine = new RingtoetsPipingSurfaceLine();
            var surfaceLines = new[]
            {
                selectedSurfaceLine,
                new RingtoetsPipingSurfaceLine()
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines))
            {
                var selectionView = (DataGridView) new ControlTester("SurfaceLineDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                cancelButton.Click();

                // Then
                Assert.IsEmpty(dialog.SelectedSurfaceLines);
            }
        }

        [Test]
        public void GivenDialogWithSelectedSurfaceLines_WhenGenerateButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedSurfaceLine = new RingtoetsPipingSurfaceLine();
            var surfaceLines = new[]
            {
                selectedSurfaceLine,
                new RingtoetsPipingSurfaceLine()
            };

            using (var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines))
            {
                var selectionView = (DataGridView) new ControlTester("SurfaceLineDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var okButton = new ButtonTester("OkButton", dialog);
                okButton.Click();

                // Then
                var result = dialog.SelectedSurfaceLines;
                CollectionAssert.AreEqual(new[]
                {
                    selectedSurfaceLine
                }, result);
            }
        }
    }
}