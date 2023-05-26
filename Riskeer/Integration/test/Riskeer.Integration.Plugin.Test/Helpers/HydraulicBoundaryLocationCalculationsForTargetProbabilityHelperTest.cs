// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Plugin.Helpers;

namespace Riskeer.Integration.Plugin.Test.Helpers
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationsForTargetProbabilityHelperTest
    {
        [Test]
        public void Create_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationsForTargetProbabilityHelper.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Create_WithoutProbability_ReturnsHydraulicBoundaryLocationCalculationsForTargetProbability()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new TestHydraulicBoundaryLocation(),
                    new TestHydraulicBoundaryLocation()
                }
            });

            // Call
            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculationsForTargetProbability =
                HydraulicBoundaryLocationCalculationsForTargetProbabilityHelper.Create(assessmentSection);

            // Assert
            Assert.AreEqual(0.01, hydraulicBoundaryLocationCalculationsForTargetProbability.TargetProbability);
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryData.GetLocations(),
                                      hydraulicBoundaryLocationCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                               .Select(c => c.HydraulicBoundaryLocation));
        }

        [Test]
        public void Create_WitProbability_ReturnsHydraulicBoundaryLocationCalculationsForTargetProbability()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new TestHydraulicBoundaryLocation(),
                    new TestHydraulicBoundaryLocation()
                }
            });

            const double probability = 0.01234;

            // Call
            HydraulicBoundaryLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculationsForTargetProbability =
                HydraulicBoundaryLocationCalculationsForTargetProbabilityHelper.Create(assessmentSection, probability);

            // Assert
            Assert.AreEqual(probability, hydraulicBoundaryLocationCalculationsForTargetProbability.TargetProbability);
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryData.GetLocations(),
                                      hydraulicBoundaryLocationCalculationsForTargetProbability.HydraulicBoundaryLocationCalculations
                                                                                               .Select(c => c.HydraulicBoundaryLocation));
        }
    }
}