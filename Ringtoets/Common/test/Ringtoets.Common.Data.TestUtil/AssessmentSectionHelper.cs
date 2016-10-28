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

using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Helper class for testing the assessment section.
    /// </summary>
    public static class AssessmentSectionHelper
    {
        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/> without the <see cref="HydraulicBoundaryDatabase"/> set.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the contribution for.</param>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        public static IAssessmentSection CreateAssessmentSectionStubWithoutBoundaryDatabase(IFailureMechanism failureMechanism,
                                                                                            MockRepository mockRepository)
        {
            return CreateAssessmentSectionStub(failureMechanism, mockRepository, false, null);
        }

        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/> with the <see cref="HydraulicBoundaryDatabase"/> set.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the contribution for.</param>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <param name="filePath">The file path to the hydraulic boundary database.</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        public static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism,
                                                                     MockRepository mockRepository,
                                                                     string filePath = null)
        {
            return CreateAssessmentSectionStub(failureMechanism, mockRepository, true, filePath);
        }

        private static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism,
                                                                      MockRepository mockRepository,
            bool addBoundaryDatabase,
                                                                      string filePath)
        {
            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.Stub(a => a.Id).Return("21");
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, 1, 2));

            if (addBoundaryDatabase)
            {
                assessmentSectionStub.HydraulicBoundaryDatabase = GetHydraulicBoundaryDatabase(filePath);
            }

            return assessmentSectionStub;
        }

        private static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(string filePath = null)
        {
            return new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Locations =
                {
                    new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0)
                }
            };
        }
    }
}