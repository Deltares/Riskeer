// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class CellStyleTest
    {
        [Test]
        public void CellStyleEnabled_Always_ReturnsEnabledStyle()
        {
            // Call
            CellStyle cellStyle = CellStyle.Enabled;

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.White), cellStyle.BackgroundColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), cellStyle.TextColor);
        }

        [Test]
        public void CellStyleDisabled_Always_ReturnsDisabledStyle()
        {
            // Call
            CellStyle cellStyle = CellStyle.Disabled;

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), cellStyle.BackgroundColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), cellStyle.TextColor);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            Color backgroundColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            Color textColor = Color.FromKnownColor(random.NextEnumValue<KnownColor>());

            // Call
            var cellStyle = new CellStyle(textColor, backgroundColor);

            // Assert
            Assert.AreEqual(backgroundColor, cellStyle.BackgroundColor);
            Assert.AreEqual(textColor, cellStyle.TextColor);
        }
    }
}