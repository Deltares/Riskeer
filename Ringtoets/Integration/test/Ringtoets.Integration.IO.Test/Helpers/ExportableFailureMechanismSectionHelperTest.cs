﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Test.Helpers
{
    [TestFixture]
    public class ExportableFailureMechanismSectionHelperTest
    {
        [Test]
        public void CreateFailureMechanismSectionResultLookup_FailureMechanismSectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup<TestFailureMechanismSectionResult>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResults", exception.ParamName);
        }

        [Test]
        public void CreateFailureMechanismSectionResultLookup_WithFailureMechanismSectionResults_ReturnsExpectedDictionary()
        {
            // Setup
            var failureMechanismSectionResults = new[]
            {
                new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(0, 10)
                })),
                new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 10),
                    new Point2D(0, 20)
                })),
                new TestFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(0, 20),
                    new Point2D(0, 40)
                }))
            };

            // Call
            IDictionary<TestFailureMechanismSectionResult, ExportableFailureMechanismSection> failureMechanismSectionResultsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanismSectionResults);

            // Assert
            CollectionAssert.AreEqual(failureMechanismSectionResults, failureMechanismSectionResultsLookup.Keys);

            TestFailureMechanismSectionResult firstSectionResult = failureMechanismSectionResults[0];
            ExportableFailureMechanismSection firstExportableSection = failureMechanismSectionResultsLookup[firstSectionResult];
            Assert.AreSame(firstSectionResult.Section.Points, firstExportableSection.Geometry);
            Assert.AreEqual(0, firstExportableSection.StartDistance);
            Assert.AreEqual(10, firstExportableSection.EndDistance);

            TestFailureMechanismSectionResult secondSectionResult = failureMechanismSectionResults[1];
            ExportableFailureMechanismSection secondExportableSection = failureMechanismSectionResultsLookup[secondSectionResult];
            Assert.AreSame(secondSectionResult.Section.Points, secondExportableSection.Geometry);
            Assert.AreEqual(10, secondExportableSection.StartDistance);
            Assert.AreEqual(20, secondExportableSection.EndDistance);

            TestFailureMechanismSectionResult thirdSectionResult = failureMechanismSectionResults[2];
            ExportableFailureMechanismSection thirdExportableSection = failureMechanismSectionResultsLookup[thirdSectionResult];
            Assert.AreSame(thirdSectionResult.Section.Points, thirdExportableSection.Geometry);
            Assert.AreEqual(20, thirdExportableSection.StartDistance);
            Assert.AreEqual(40, thirdExportableSection.EndDistance);
        }
    }
}