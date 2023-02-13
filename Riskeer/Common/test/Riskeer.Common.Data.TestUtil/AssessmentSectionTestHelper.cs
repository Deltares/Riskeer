﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating assessment sections that can be used for unit tests.
    /// </summary>
    public static class AssessmentSectionTestHelper
    {
        private static readonly RoundedDouble testAssessmentLevel = new Random(21).NextRoundedDouble();

        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/> with a <see cref="HydraulicBoundaryDatabase"/> that is not linked.
        /// </summary>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        public static IAssessmentSection CreateAssessmentSectionStub(MockRepository mockRepository)
        {
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            assessmentSection.Replay();

            return assessmentSection;
        }

        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the contribution for.</param>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <param name="filePath">The file path to the hydraulic boundary database (optional).</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        /// <remarks>Whether <paramref name="filePath"/> is provided or not, a dummy location with id 1300001 is added to the
        /// hydraulic boundary database.</remarks>
        public static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism,
                                                                     MockRepository mockRepository,
                                                                     string filePath = null)
        {
            IFailureMechanism[] failureMechanisms = GetFailureMechanisms(failureMechanism);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("21");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(0.1, 1.0 / 30000));
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(GetHydraulicBoundaryDatabase(filePath));
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            assessmentSection.Replay();

            return assessmentSection;
        }

        /// <summary>
        /// Gets a random assessment level for testing purposes.
        /// </summary>
        /// <returns>The assessment level.</returns>
        /// <remarks>The returned assessment level is random, though always the same.</remarks>
        public static RoundedDouble GetTestAssessmentLevel()
        {
            return testAssessmentLevel;
        }

        private static IFailureMechanism[] GetFailureMechanisms(IFailureMechanism failureMechanism)
        {
            return failureMechanism == null
                       ? Enumerable.Empty<IFailureMechanism>().ToArray()
                       : new[]
                       {
                           failureMechanism
                       };
        }

        private static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(string filePath)
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Locations =
                {
                    new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0, new HrdFile())
                }
            };

            if (filePath != null)
            {
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);
            }

            return hydraulicBoundaryDatabase;
        }
    }
}