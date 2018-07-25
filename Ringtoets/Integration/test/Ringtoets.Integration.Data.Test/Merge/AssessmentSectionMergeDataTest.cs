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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Data.Test.Merge
{
    [TestFixture]
    public class AssessmentSectionMergeDataTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(null, Enumerable.Empty<IFailureMechanism>(),
                                                                     new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                     null, new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void Constructor_PropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionMergeData(new AssessmentSection(AssessmentSectionComposition.Dike),
                                                                     Enumerable.Empty<IFailureMechanism>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            // Call
            var mergeData = new AssessmentSectionMergeData(assessmentSection, failureMechanisms,
                                                           new AssessmentSectionMergeData.ConstructionProperties());

            // Assert
            Assert.AreSame(assessmentSection, mergeData.AssessmentSection);
            Assert.AreSame(failureMechanisms, mergeData.FailureMechanisms);
        }
    }
}