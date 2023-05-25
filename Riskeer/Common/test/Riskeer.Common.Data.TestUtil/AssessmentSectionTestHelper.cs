// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Helper class for creating assessment sections that can be used for unit tests.
    /// </summary>
    public static class AssessmentSectionTestHelper
    {
        private static readonly RoundedDouble testAssessmentLevel = new Random(21).NextRoundedDouble();

        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/> with <see cref="HydraulicBoundaryData"/> that is not linked.
        /// </summary>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        public static IAssessmentSection CreateAssessmentSectionStub(MockRepository mockRepository)
        {
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(new HydraulicBoundaryData());
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            assessmentSection.Replay();

            return assessmentSection;
        }

        /// <summary>
        /// Creates a stub of <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the contribution for.</param>
        /// <param name="mockRepository">The mock repository to create the stub with.</param>
        /// <param name="hrdFilePath">The path to the hydraulic boundary database (optional).</param>
        /// <param name="usePreprocessorClosure">Whether or not to use preprocessor closure (optional).</param>
        /// <returns>A stubbed <see cref="IAssessmentSection"/>.</returns>
        /// <remarks>When a <paramref name="hrdFilePath"/> is provided:
        /// <list type="bullet">
        /// <item>the hydraulic location configuration database file path is set automatically (to a file in the same directory with name 'hlcd.sqlite');</item>
        /// <item>a dummy location with id 1300001 is added to the hydraulic boundary database.</item>
        /// </list>
        /// </remarks>
        public static IAssessmentSection CreateAssessmentSectionStub(IFailureMechanism failureMechanism,
                                                                     MockRepository mockRepository,
                                                                     string hrdFilePath = null,
                                                                     bool usePreprocessorClosure = false)
        {
            IFailureMechanism[] failureMechanisms = GetFailureMechanisms(failureMechanism);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("21");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(0.1, 1.0 / 30000));
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(GetHydraulicBoundaryData(hrdFilePath, usePreprocessorClosure));
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

        /// <summary>
        /// Gets all the generic and specific failure mechanisms of the <see cref="IAssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the failure mechanisms for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFailureMechanism"/>.</returns>
        public static IEnumerable<IFailureMechanism> GetAllFailureMechanisms(IAssessmentSection assessmentSection)
        {
            return assessmentSection.GetFailureMechanisms()
                                    .Concat(assessmentSection.SpecificFailureMechanisms)
                                    .ToArray();
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

        private static HydraulicBoundaryData GetHydraulicBoundaryData(string hrdFilePath, bool usePreprocessorClosure)
        {
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            if (hrdFilePath != null)
            {
                hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = Path.Combine(Path.GetDirectoryName(hrdFilePath), "hlcd.sqlite");

                hydraulicBoundaryData.HydraulicBoundaryDatabases.Add(new HydraulicBoundaryDatabase
                {
                    FilePath = hrdFilePath,
                    Version = GetVersion(hrdFilePath),
                    UsePreprocessorClosure = usePreprocessorClosure,
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0)
                    }
                });
            }

            return hydraulicBoundaryData;
        }

        private static string GetVersion(string hrdFilePath)
        {
            if (!File.Exists(hrdFilePath))
            {
                return null;
            }

            using (var db = new HydraulicBoundaryDatabaseReader(hrdFilePath))
            {
                return db.ReadVersion();
            }
        }
    }
}