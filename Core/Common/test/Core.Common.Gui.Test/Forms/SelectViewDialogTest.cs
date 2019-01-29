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
using System.Windows.Forms;
using Core.Common.Gui.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms
{
    [TestFixture]
    public class SelectViewDialogTest
    {
        [Test]
        public void Constructor_WithoutDialogParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new SelectViewDialog(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", paramName);
        }

        [Test]
        public void Constructor_WithDialogParent_SetProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var parent = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            using (var dialog = new SelectViewDialog(parent))
            {
                // Assert
                Assert.IsNull(dialog.DefaultViewName);
                Assert.IsNull(dialog.SelectedItem);
                Assert.IsNull(dialog.Items);
            }

            mocks.VerifyAll();
        }
    }
}