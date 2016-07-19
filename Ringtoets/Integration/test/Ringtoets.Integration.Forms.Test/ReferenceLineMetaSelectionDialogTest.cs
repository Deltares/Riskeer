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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Dialogs;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class ReferenceLineMetaSelectionDialogTest : NUnitFormTest
    {
        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReferenceLineMetaSelectionDialog(null, Enumerable.Empty<ReferenceLineMeta>());

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutReferenceLineMetas_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReferenceLineMetaSelectionDialog(new Form(), null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("referenceLineMetas", parameter);
        }

        [Test]
        public void Constructor_WithParentAndReferenceLineMeta_DefaultProperties()
        {
            // Call
            using (var dialog = new ReferenceLineMetaSelectionDialog(new Form(), Enumerable.Empty<ReferenceLineMeta>()))
            {
                // Assert
                Assert.IsInstanceOf<DialogBase>(dialog);
                Assert.IsNull(dialog.SelectedReferenceLineMeta);
                Assert.AreEqual("Selecteer referentielijn", dialog.Text);

                AssertSignalingLowerLimitComboBox(dialog);
                AssertReferenceLineMetaDataGridViewControl(dialog);
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var dialog = new ReferenceLineMetaSelectionDialog(new Form(), Enumerable.Empty<ReferenceLineMeta>()))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(410, dialog.MinimumSize.Width);
                Assert.AreEqual(350, dialog.MinimumSize.Height);
            }
        }

        private static void AssertReferenceLineMetaDataGridViewControl(ReferenceLineMetaSelectionDialog dialog)
        {
            var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", dialog).TheObject;
            Assert.IsFalse(grid.MultiSelect);
            Assert.AreEqual(DataGridViewSelectionMode.FullRowSelect, grid.SelectionMode);
        }

        private static void AssertSignalingLowerLimitComboBox(ReferenceLineMetaSelectionDialog dialog)
        {
            var combobox = (ComboBox) new ComboBoxTester("SignalingLowerLimitComboBox", dialog).TheObject;
            Assert.AreEqual(ComboBoxStyle.DropDownList, combobox.DropDownStyle);
        }
    }
}