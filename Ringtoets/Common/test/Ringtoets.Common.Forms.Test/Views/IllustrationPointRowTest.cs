// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class IllustrationPointRowTest
    {
        [Test]
        public void Constructor_WindDirectionNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointRow(null, string.Empty, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("windDirection", exception.ParamName);
        }

        [Test]
        public void Constructor_ClosingSituationNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointRow("South", null, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("closingSituation", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string windDirection = "South";
            const string closingSituation = "Regular";
            const double probability = 0.2;
            const double reliability = 0.1;

            // Call
            var row = new IllustrationPointRow(windDirection, closingSituation, probability, reliability);

            // Assert
            Assert.AreEqual(windDirection, row.WindDirection);
            Assert.AreEqual(closingSituation, row.ClosingSituation);
            Assert.AreEqual(probability, row.Probability);
            Assert.AreEqual(reliability, row.Reliability);
        }
    }
}