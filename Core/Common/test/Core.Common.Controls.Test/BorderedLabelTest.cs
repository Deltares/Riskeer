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

using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.Test
{
    [TestFixture]
    public class BorderedLabelTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var borderedLabel = new BorderedLabel();

            // Assert
            Assert.IsInstanceOf<Label>(borderedLabel);
            Assert.IsTrue(borderedLabel.AutoSize);
            Assert.AreEqual(BorderStyle.FixedSingle, borderedLabel.BorderStyle);
            Assert.AreEqual(DockStyle.Fill, borderedLabel.Dock);
            Assert.AreEqual(new Size(50, 0), borderedLabel.MinimumSize);
            Assert.AreEqual(new Padding(5, 0, 5, 0), borderedLabel.Padding);
            Assert.AreEqual(ContentAlignment.MiddleLeft, borderedLabel.TextAlign);
        }
    }
}