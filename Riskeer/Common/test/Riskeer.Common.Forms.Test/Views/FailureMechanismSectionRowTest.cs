﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionRowTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismSectionRow(null, double.NaN, double.NaN);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanismSection_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();

            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(0.0, 10.0)
            });

            // Call
            var sectionRow = new FailureMechanismSectionRow(section, sectionStart, sectionEnd);

            // Assert
            Assert.AreEqual(section.Name, sectionRow.Name);
            Assert.AreEqual(2, sectionRow.Length.NumberOfDecimalPlaces);
            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());
            Assert.AreEqual(2, sectionRow.SectionStart.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionStart, sectionRow.SectionStart, sectionRow.SectionStart.GetAccuracy());
            Assert.AreEqual(2, sectionRow.SectionEnd.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionEnd, sectionRow.SectionEnd, sectionRow.SectionEnd.GetAccuracy());
        }
    }
}