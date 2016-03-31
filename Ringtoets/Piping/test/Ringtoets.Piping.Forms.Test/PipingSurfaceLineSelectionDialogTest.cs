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
            var dialog = new PipingSurfaceLineSelectionDialog(new Form(), Enumerable.Empty<RingtoetsPipingSurfaceLine>());

            // Assert
            Assert.IsEmpty(dialog.SelectedSurfaceLines);
            Assert.IsInstanceOf<PipingSurfaceLineSelectionView>(new ControlTester("PipingSurfaceLineSelectionView", dialog).TheObject);
            Assert.AreEqual("Selecteer profielschematisaties", dialog.Text);
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            var dialog = new PipingSurfaceLineSelectionDialog(new Form(), Enumerable.Empty<RingtoetsPipingSurfaceLine>());

            // Call
            dialog.Show();

            // Assert
            Assert.AreEqual(300, dialog.MinimumSize.Width);
            Assert.AreEqual(400, dialog.MinimumSize.Height);
        }

        [Test]
        public void SelectedSurfaceLines_SurfaceLinesSelectedInSelectionViewCloseWithoutConfirmation_ReturnsEmptyCollection()
        {
            // Setup
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines);

            var selectionView = (DataGridView)new ControlTester("SurfaceLineDataGrid", dialog).TheObject;

            dialog.Show();
            selectionView.Rows[0].Cells[0].Value = true;
            dialog.Close();

            // Call
            var result = dialog.SelectedSurfaceLines;

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void SelectedSurfaceLines_SurfaceLinesSelectedInSelectionViewCancelButtonClicked_ReturnsSelectedCollection()
        {
            // Setup
            var selectedSurfaceLine = new RingtoetsPipingSurfaceLine();
            var surfaceLines = new[]
            {
                selectedSurfaceLine,
                new RingtoetsPipingSurfaceLine()
            };

            var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines);
            var selectionView = (DataGridView)new ControlTester("SurfaceLineDataGrid", dialog).TheObject;
            var cancelButton = new ButtonTester("CancelButton", dialog);

            dialog.Show();
            selectionView.Rows[0].Cells[0].Value = true;
            cancelButton.Click();

            // Call
            var result = dialog.SelectedSurfaceLines;

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void SelectedSurfaceLines_SurfaceLinesSelectedInSelectionGenerateButtonClicked_ReturnsSelectedCollection()
        {
            // Setup
            var selectedSurfaceLine = new RingtoetsPipingSurfaceLine();
            var surfaceLines = new[]
            {
                selectedSurfaceLine,
                new RingtoetsPipingSurfaceLine()
            };

            var dialog = new PipingSurfaceLineSelectionDialog(new Form(), surfaceLines);
            var selectionView = (DataGridView)new ControlTester("SurfaceLineDataGrid", dialog).TheObject;
            var okButton = new ButtonTester("OkButton", dialog);

            dialog.Show();
            selectionView.Rows[0].Cells[0].Value = true;
            okButton.Click();

            // Call
            var result = dialog.SelectedSurfaceLines;

            // Assert
            CollectionAssert.AreEqual(new [] {selectedSurfaceLine},result);
        }
    }
}