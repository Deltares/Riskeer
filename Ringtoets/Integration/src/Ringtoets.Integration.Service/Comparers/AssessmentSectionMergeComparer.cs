// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Riskeer.Integration.Data;

namespace Ringtoets.Integration.Service.Comparers
{
    /// <summary>
    /// Class which compares <see cref="AssessmentSection"/> to
    /// determine whether they are equal and can be used to merged.
    /// </summary>
    public class AssessmentSectionMergeComparer : IAssessmentSectionMergeComparer
    {
        public bool Compare(AssessmentSection assessmentSection, AssessmentSection otherAssessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (otherAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(otherAssessmentSection));
            }

            return assessmentSection.Id == otherAssessmentSection.Id
                   && assessmentSection.Composition == otherAssessmentSection.Composition
                   && AreReferenceLinesEquivalent(assessmentSection.ReferenceLine, otherAssessmentSection.ReferenceLine)
                   && AreHydraulicBoundaryDatabasesEquivalent(assessmentSection.HydraulicBoundaryDatabase, otherAssessmentSection.HydraulicBoundaryDatabase)
                   && AreFailureMechanismContributionsEquivalent(assessmentSection.FailureMechanismContribution, otherAssessmentSection.FailureMechanismContribution);
        }

        private static bool AreReferenceLinesEquivalent(ReferenceLine referenceLine, ReferenceLine otherReferenceLine)
        {
            IEnumerable<Point2D> referenceLineGeometry = referenceLine.Points;
            IEnumerable<Point2D> otherReferenceLineGeometry = otherReferenceLine.Points;

            int nrOfPoints = referenceLineGeometry.Count();
            if (otherReferenceLineGeometry.Count() != nrOfPoints)
            {
                return false;
            }

            for (var i = 0; i < nrOfPoints; i++)
            {
                if (!referenceLineGeometry.ElementAt(i).Equals(otherReferenceLineGeometry.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreHydraulicBoundaryDatabasesEquivalent(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                                    HydraulicBoundaryDatabase otherHydraulicBoundaryDatabase)
        {
            return hydraulicBoundaryDatabase.Version == otherHydraulicBoundaryDatabase.Version;
        }

        private static bool AreFailureMechanismContributionsEquivalent(FailureMechanismContribution failureMechanismContribution,
                                                                       FailureMechanismContribution otherFailureMechanismContribution)
        {
            return AreNormsEquivalent(failureMechanismContribution.LowerLimitNorm, otherFailureMechanismContribution.LowerLimitNorm)
                   && AreNormsEquivalent(failureMechanismContribution.SignalingNorm, otherFailureMechanismContribution.SignalingNorm)
                   && failureMechanismContribution.NormativeNorm == otherFailureMechanismContribution.NormativeNorm;
        }

        private static bool AreNormsEquivalent(double norm, double otherNorm)
        {
            return Math.Abs(norm - otherNorm) < double.Epsilon;
        }
    }
}