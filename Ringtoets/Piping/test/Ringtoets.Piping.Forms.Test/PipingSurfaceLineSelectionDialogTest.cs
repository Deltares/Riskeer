using System;
using System.Collections.Generic;
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
                var cancelButton = new ButtonTester("CancelButton", dialog);
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