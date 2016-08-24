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
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
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
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, null);

                // Assert
                var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("dikeProfiles", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndDikeProfiles_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, Enumerable.Empty<DikeProfile>()))
                {
                    // Assert
                    Assert.IsEmpty(dialog.SelectedDikeProfiles);
                    Assert.IsInstanceOf<GrassCoverErosionInwardsDikeProfileSelectionView>(new ControlTester("GrassCoverErosionInwardsDikeProfileSelectionView", dialog).TheObject);
                    Assert.AreEqual("Selecteer dijkprofielen", dialog.Text);
                }
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, Enumerable.Empty<DikeProfile>()))
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
                CreateDikeProfile(),
                CreateDikeProfile()
            };

            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, dikeProfiles))
            {
                var selectionView = (DataGridView) new ControlTester("DikeProfileDataGrid", dialog).TheObject;

                dialog.Show();
                selectionView.Rows[0].Cells[0].Value = true;

                // When
                dialog.Close();

                // Then
                Assert.IsEmpty(dialog.SelectedDikeProfiles);
            }
        }

        [Test]
        public void GivenDialogWithSelectedDikeProfiles_WhenCancelButtonClicked_ThenReturnsSelectedCollection()
        {
            // Given
            var selectedDikeProfile = CreateDikeProfile();
            var dikeProfiles = new[]
            {
                selectedDikeProfile,
                CreateDikeProfile()
            };

            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, dikeProfiles))
            {
                var selectionView = (DataGridView) new ControlTester("DikeProfileDataGrid", dialog).TheObject;

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
            var selectedDikeProfile = CreateDikeProfile();
            var dikeProfiles = new[]
            {
                selectedDikeProfile,
                CreateDikeProfile()
            };

            using (var viewParent = new Form())
            using (var dialog = new GrassCoverErosionInwardsDikeProfileSelectionDialog(viewParent, dikeProfiles))
            {
                var selectionView = (DataGridView) new ControlTester("DikeProfileDataGrid", dialog).TheObject;

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

        private DikeProfile CreateDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                   null, new DikeProfile.ConstructionProperties());
        }
    }
}