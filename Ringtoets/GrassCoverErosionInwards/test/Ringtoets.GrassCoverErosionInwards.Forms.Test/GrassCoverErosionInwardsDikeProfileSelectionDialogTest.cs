using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsDikeProfileSelectionDialogTest
    {
        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionDialog(null, Enumerable.Empty<DikeProfile>());

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutDikeProfiles_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dikeProfiles", parameter);
        }

        [Test]
        public void Constructor_WithParentAndDikeProfiles_DefaultProperties()
        {
            // Call
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), Enumerable.Empty<DikeProfile>()))
            {

                // Assert
                Assert.IsEmpty(dialog.SelectedDikeProfiles);
                Assert.IsInstanceOf<GrassCoverErosionInwardsDikeProfileSelectionView>(new ControlTester("GrassCoverErosionInwardsDikeProfileSelectionView", dialog).TheObject);
                Assert.AreEqual("Selecteer dijkprofielen", dialog.Text);
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), Enumerable.Empty<DikeProfile>()))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(300, dialog.MinimumSize.Width);
                Assert.AreEqual(400, dialog.MinimumSize.Height);
            }
        }

        [Test]
        public void GivenDialogWithSelectedDikeProfiles_WhenCloseWithoutConfirmation_ThenReturnsEmptyCollection()
        {
            // Given
            var dikeProfiles = new[]
            {
                new DikeProfile(CreateTestPoint()),
                new DikeProfile(CreateTestPoint())
            };

            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), dikeProfiles))
            {

                var selectionView = (DataGridView)new ControlTester("DikeProfileDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                dialog.Close();

                // Then
                Assert.IsEmpty(dialog.SelectedDikeProfiles);
            }
        }

        private Point2D CreateTestPoint()
        {
            return new Point2D(0,0);
        }

        [Test]
        public void GivenDialogWithSelectedDikeProfiles_WhenCancelButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedDikeProfile = new DikeProfile(CreateTestPoint());
            var dikeProfiles = new[]
            {
                selectedDikeProfile,
                new DikeProfile(CreateTestPoint())
            };

            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), dikeProfiles))
            {
                var selectionView = (DataGridView)new ControlTester("DikeProfileDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var cancelButton = new ButtonTester("CustomCancelButton", dialog);
                cancelButton.Click();

                // Then
                Assert.IsEmpty(dialog.SelectedDikeProfiles);
            }
        }

        [Test]
        public void GivenDialogWithSelectedDikeProfiles_WhenGenerateButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedDikeProfile = new DikeProfile(CreateTestPoint());
            var dikeProfiles = new[]
            {
                selectedDikeProfile,
                new DikeProfile(CreateTestPoint())
            };

            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(new Form(), dikeProfiles))
            {
                var selectionView = (DataGridView)new ControlTester("DikeProfileDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                var okButton = new ButtonTester("OkButton", dialog);
                okButton.Click();

                // Then
                var result = dialog.SelectedDikeProfiles;
                CollectionAssert.AreEqual(new[]
                {
                    selectedDikeProfile
                }, result);
            }
        } 
    }
}