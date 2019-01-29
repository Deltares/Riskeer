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
using System.Drawing;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class StrictContextMenuItemTest
    {
        [Test]
        public void Constructor_WithParameters_PropertiesSet()
        {
            // Setup
            var mockRepository = new MockRepository();

            const string text = "text";
            const string toolTip = "tooltip";
            Bitmap image = Resources.ImportIcon;
            var counter = 0;

            mockRepository.ReplayAll();

            EventHandler handler = (s, e) => counter++;

            // Call
            var result = new StrictContextMenuItem(text, toolTip, image, handler);
            result.PerformClick();

            // Assert
            Assert.IsInstanceOf<StrictContextMenuItem>(result);
            Assert.AreEqual(text, result.Text);
            Assert.AreEqual(toolTip, result.ToolTipText);
            Assert.AreEqual(1, counter);
            TestHelper.AssertImagesAreEqual(image, result.Image);

            mockRepository.VerifyAll();
        }
    }
}