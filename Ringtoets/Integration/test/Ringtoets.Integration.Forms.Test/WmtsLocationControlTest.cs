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

using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Ringtoets.Integration.Forms.Test
{
    [TestFixture]
    public class WmtsLocationControlTest : NUnitFormTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var control = new WmtsLocationControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(control);
            }
        }

        [Test]
        public void Show_ControlAddedToForm_DefaultProperties()
        {
            // Setup
            using (var form = new Form())
            using (var control = new WmtsLocationControl())
            {
                form.Controls.Add(control);

                // Call
                form.Show();

                // Assert
                var urlLocationLabel = new LabelTester("urlLocationLabel", form);
                Assert.AreEqual("Locatie (URL)", urlLocationLabel.Text);

                var urlLocations = new ComboBoxTester("urlLocationComboBox", form);
                Assert.IsNotNull(urlLocations);

                var buttonConnectTo = new ButtonTester("connectToButton", form);
                Assert.AreEqual("Verbinding maken", buttonConnectTo.Text);

                var buttonAddLocation = new ButtonTester("addLocationButton", form);
                Assert.AreEqual("Locatie toevoegen...", buttonAddLocation.Text);

                var buttonEditLocation = new ButtonTester("editLocationButton", form);
                Assert.AreEqual("Locatie aanpassen...", buttonEditLocation.Text);
            }
        }
    }
}