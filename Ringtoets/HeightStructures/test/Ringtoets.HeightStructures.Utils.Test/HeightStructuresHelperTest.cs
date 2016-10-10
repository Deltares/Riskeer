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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.HeightStructures.Utils.Test
{
    [TestFixture]
    public class HeightStructuresHelperTest
    {
        private const string firstSectionName = "firstSection";
        private const string secondSectionName = "secondSection";

        private readonly FailureMechanismSection[] oneSection =
        {
            new FailureMechanismSection("testFailureMechanismSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0)
            })
        };

        private readonly FailureMechanismSection[] twoSections =
        {
            new FailureMechanismSection(firstSectionName, new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 10.0),
            }),
            new FailureMechanismSection(secondSectionName, new List<Point2D>
            {
                new Point2D(11.0, 11.0),
                new Point2D(100.0, 100.0),
            })
        };

        [Test]
        public void FailureMechanismSectionForCalculation_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new HeightStructuresCalculation();

            // Call
            TestDelegate call = () => HeightStructuresHelper.FailureMechanismSectionForCalculation(null, calculation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sections", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresHelper.FailureMechanismSectionForCalculation(oneSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_ValidSectionWithoutCalculationStructureSet_ReturnsNull()
        {
            // Setup
            var calculation = new HeightStructuresCalculation();

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(oneSection, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_EmptySectionWithoutCalculationStructureSet_ReturnsNull()
        {
            // Setup
            var emptySections = new FailureMechanismSection[0];
            var calculation = new HeightStructuresCalculation();

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(emptySections, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_FirstSectionContainsCalculation_FailureMechanismSectionOfFirstSection()
        {
            // Setup
            var calculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HeightStructure = new TestHeightStructure("test", new Point2D(1.1, 2.2))
                }
            };

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(twoSections, calculation);

            // Assert
            Assert.AreSame(twoSections[0], failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SecondSectionContainsCalculation_FailureMechanismSectionOfSecondSection()
        {
            // Setup
            var calculation = new HeightStructuresCalculation
            {
                InputParameters =
                {
                    HeightStructure = new TestHeightStructure("test", new Point2D(55.0, 66.0))
                }
            };

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(twoSections, calculation);

            // Assert
            Assert.AreSame(twoSections[1], failureMechanismSection);
        }
    }
}