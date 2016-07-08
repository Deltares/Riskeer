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

using System.Windows.Forms;
using Core.Common.Gui.Forms.Options;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.Options
{
    [TestFixture]
    public class OptionsDialogTest
    {
        [Test]
        public void GivenGeneralOptionsControlCreated_WhenRetrievingOptionsItems_ThenOptionsShouldBeTranslated()
        {
            // Setup
            var mocks = new MockRepository();
            var window = mocks.Stub<IWin32Window>();

            mocks.ReplayAll();

            using (var control = new OptionsDialog(window, null))
            {
                var subControl = (ComboBox) control.Controls.Find("comboBoxTheme", true)[0];

                Assert.AreEqual(6, subControl.Items.Count);

                var localizedThemes = new[]
                {
                    "Donker",
                    "Licht",
                    "Metro",
                    "Aero",
                    "VS2010",
                    "Generiek"
                };

                for (var i = 0; i < subControl.Items.Count; i++) 
                {
                    Assert.AreEqual(localizedThemes[i], subControl.GetItemText(subControl.Items[i]));
                }
            }
        }
    }
}