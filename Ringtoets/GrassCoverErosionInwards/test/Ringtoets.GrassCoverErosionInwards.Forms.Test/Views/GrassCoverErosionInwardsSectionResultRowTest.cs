﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsSectionResultRowTest
    {
        [Test]
        public void Constructor_ValidValue_ExpectedValues()
        {
            // Setup
            var section = new FailureMechanismSection("haha", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new GrassCoverErosionInwardsSectionResultRow(sectionResult);

            // Assert
            Assert.AreEqual(sectionResult.Section.Name, row.Name);
            Assert.AreEqual(sectionResult.Calculation, row.Calculation);
        }

        [Test]
        public void Constructor_SectionResultIsNull_ThrowArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsSectionResultRow(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Calculation_SetNewValue_UpdatesSectionResultCalculation()
        {
            // Setup
            var section = new FailureMechanismSection("haha", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            var row = new GrassCoverErosionInwardsSectionResultRow(sectionResult);

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            row.Calculation = calculation;

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreSame(calculation, sectionResult.Calculation);
        }
    }
}