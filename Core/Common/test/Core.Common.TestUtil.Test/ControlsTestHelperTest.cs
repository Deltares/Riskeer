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

using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class ControlsTestHelperTest
    {
        [Test]
        public void FakeUserSelectingNewValue_ValueInComboBoxDifferentFromCurrentValue_SetSelectedValueAndFireEvents()
        {
            // Setup
            const string selectedindexchanged = "SelectedIndexChanged";
            const string selectedvaluechanged = "SelectedValueChanged";
            const string selectionchangecommitted = "SelectionChangeCommitted";

            const string value1 = "1";
            const string value2 = "2";

            using (var form = new Form())
            using (var control = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = new[]
                {
                    Tuple.Create(value1, "A"),
                    Tuple.Create(value2, "B")
                },
                ValueMember = "Item1",
                DisplayMember = "Item2"
            })
            {
                control.SelectedValue = value1;

                form.Controls.Add(control);
                form.Show();

                var raisedEvents = new Collection<string>();
                control.SelectedIndexChanged += (sender, args) => raisedEvents.Add(selectedindexchanged);
                control.SelectedValueChanged += (sender, args) => raisedEvents.Add(selectedvaluechanged);
                control.SelectionChangeCommitted += (sender, args) => raisedEvents.Add(selectionchangecommitted);
                control.Validating += (sender, args) => Assert.Fail("Validating event should not be fired as 'FakeUserSelectingNewValue' method does not handle focus-changes.");
                control.Validated += (sender, args) => Assert.Fail("Validated event should not be fired as 'FakeUserSelectingNewValue' method does not handle focus-changes.");

                // Call
                ControlsTestHelper.FakeUserSelectingNewValue(control, value2);

                // Assert
                Assert.AreEqual(value2, control.SelectedValue);
                string[] expectedRaisedEvents =
                {
                    selectedvaluechanged,
                    selectedindexchanged,
                    selectionchangecommitted
                };
                CollectionAssert.AreEqual(expectedRaisedEvents, raisedEvents);
            }
        }
    }
}