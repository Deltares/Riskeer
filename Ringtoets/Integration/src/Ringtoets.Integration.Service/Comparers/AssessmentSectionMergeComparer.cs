﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Integration.Service.Comparers
{
    /// <summary>
    /// Class which compares <see cref="IAssessmentSection"/> to
    /// determine whether they are equal and can be used to merged.
    /// </summary>
    public class AssessmentSectionMergeComparer : IAssessmentSectionMergeComparer
    {
        public bool Compare(IAssessmentSection referenceAssessmentSection, IAssessmentSection assessmentSectionToCompare)
        {
            if (referenceAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(referenceAssessmentSection));
            }

            if (assessmentSectionToCompare == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionToCompare));
            }

            return referenceAssessmentSection.Id == assessmentSectionToCompare.Id
                   && referenceAssessmentSection.Composition == assessmentSectionToCompare.Composition
                   && AreReferenceLinesEquivalent(referenceAssessmentSection.ReferenceLine, assessmentSectionToCompare.ReferenceLine)
                   && AreHydraulicBoundaryDatabasesEquivalent(referenceAssessmentSection.HydraulicBoundaryDatabase, assessmentSectionToCompare.HydraulicBoundaryDatabase)
                   && AreFailureMechanismContributionsEquivalent(referenceAssessmentSection.FailureMechanismContribution, assessmentSectionToCompare.FailureMechanismContribution);
        }

        private static bool AreReferenceLinesEquivalent(ReferenceLine referenceLine, ReferenceLine otherReferenceLine)
        {
            if (referenceLine == null && otherReferenceLine == null)
            {
                return true;
            }

            IEnumerable<Point2D> referenceLineGeometry = referenceLine?.Points;
            IEnumerable<Point2D> otherReferenceLineGeometry = otherReferenceLine?.Points;

            if (referenceLineGeometry == null || otherReferenceLineGeometry == null)
            {
                return false;
            }

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
            return Math.Abs(norm - otherNorm) < 1e-5;
        }
    }
}