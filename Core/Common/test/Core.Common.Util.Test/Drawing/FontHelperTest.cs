// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing.Text;
using Core.Common.Util.Drawing;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test.Drawing
{
    [TestFixture]
    public class FontHelperTest
    {
        [Test]
        public void CreateFont_ValidFontData_CreatesExpectedFont()
        {
            // Setup
            var privateFontCollection = new PrivateFontCollection();

            // Call
            Font font = FontHelper.CreateFont(Resources.ValidFont, privateFontCollection);

            // Assert
            Assert.IsNotNull(font);
            Assert.AreEqual(14, font.Size);
            Assert.AreEqual(1, privateFontCollection.Families.Length);
        }

        [Test]
        public void CreateFont_FontDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FontHelper.CreateFont(null, new PrivateFontCollection());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("fontData", exception.ParamName);
        }

        [Test]
        public void CreateFont_PrivateFontCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FontHelper.CreateFont(Resources.ValidFont, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("privateFontCollection", exception.ParamName);
        }
    }
}