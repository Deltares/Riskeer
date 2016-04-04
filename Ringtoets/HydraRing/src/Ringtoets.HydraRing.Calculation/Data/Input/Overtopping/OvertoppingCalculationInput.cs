// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;

namespace Ringtoets.HydraRing.Calculation.Data.Input.Overtopping
{
    /// <summary>
    /// Container of all data necessary for performing an overtopping calculation via Hydra-Ring.
    /// </summary>
    public class OvertoppingCalculationInput : ExceedanceProbabilityCalculationInput
    {
        private readonly HydraRingSection section;
        private readonly IEnumerable<HydraRingProfilePoint> profilePoints;
        private readonly IEnumerable<HydraRingForelandPoint> forelandPoints;
        private readonly IEnumerable<HydraRingBreakWater> breakWaters;

        private readonly double dikeHeight;
        private readonly double criticalOvertoppingMean;
        private readonly double criticalOvertoppingStandardDeviation;

        /// <summary>
        /// Creates a new instance of the <see cref="OvertoppingCalculationInput"/> class.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="hydraRingSection">The section to use during the calculation.</param>
        /// <param name="hydraRingDikeHeight">The dike height to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingMean">The mean of the critical overtopping to use during the calculation.</param>
        /// <param name="hydraRingCriticalOvertoppingStandardDeviation">The standard deviation of the critical overtopping to use during the calculation.</param>
        /// <param name="hydraRingProfilePoints">The profile points to use during the calculation.</param>
        /// <param name="hydraRingForelandPoints">The foreland points to use during the calculation.</param>
        /// <param name="hydraRingBreakWaters">The break water to use during the calculation.</param>
        public OvertoppingCalculationInput(int hydraulicBoundaryLocationId, HydraRingSection hydraRingSection,
                                           double hydraRingDikeHeight, double hydraRingCriticalOvertoppingMean,
                                           double hydraRingCriticalOvertoppingStandardDeviation,
                                           IEnumerable<HydraRingRoughnessProfilePoint> hydraRingProfilePoints,
                                           IEnumerable<HydraRingForelandPoint> hydraRingForelandPoints,
                                           IEnumerable<HydraRingBreakWater> hydraRingBreakWaters)
            : base(hydraulicBoundaryLocationId)
        {
            section = hydraRingSection;
            dikeHeight = hydraRingDikeHeight;
            criticalOvertoppingMean = hydraRingCriticalOvertoppingMean;
            criticalOvertoppingStandardDeviation = hydraRingCriticalOvertoppingStandardDeviation;
            profilePoints = hydraRingProfilePoints;
            forelandPoints = hydraRingForelandPoints;
            breakWaters = hydraRingBreakWaters;
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.DikesOvertopping;
            }
        }

        public override int VariableId
        {
            get
            {
                return 1;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return section;
            }
        }

        public override IEnumerable<HydraRingProfilePoint> ProfilePoints
        {
            get
            {
                return profilePoints;
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandsPoints
        {
            get
            {
                return forelandPoints;
            }
        }

        public override IEnumerable<HydraRingBreakWater> BreakWaters
        {
            get
            {
                return breakWaters;
            }
        }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new OvertoppingVariableDikeHeight(dikeHeight);
                yield return new OvertoppingVariableModelFactorCriticalOvertopping();
                yield return new OvertoppingVariableFactorFb();
                yield return new OvertoppingVariableFactorFn();
                yield return new OvertoppingVariableModelFactorOvertopping();
                yield return new OvertoppingVariableCriticalOvertopping(criticalOvertoppingMean, criticalOvertoppingStandardDeviation);
                yield return new OvertoppingVariableModelFactorFrunup();
                yield return new OvertoppingVariableExponentModelFactorShallow();
            }
        }

        public override int? GetSubMechanismModelId(int subMechanismId)
        {
            switch (subMechanismId)
            {
                case 102:
                    return 94;
                case 103:
                    return 95;
                default:
                    return null;
            }
        }

        #region Overtopping Variables

        private class OvertoppingVariableDikeHeight : HydraRingVariable
        {
            public OvertoppingVariableDikeHeight(double dikeHeight) : base(1, HydraRingDistributionType.Deterministic, dikeHeight, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingVariableModelFactorCriticalOvertopping : HydraRingVariable
        {
            public OvertoppingVariableModelFactorCriticalOvertopping() : base(8, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingVariableFactorFb : HydraRingVariable
        {
            public OvertoppingVariableFactorFb() : base(10, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 4.75, 0.5, double.NaN) {}
        }

        private class OvertoppingVariableFactorFn : HydraRingVariable
        {
            public OvertoppingVariableFactorFn() : base(11, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 2.60, 0.35, double.NaN) {}
        }

        private class OvertoppingVariableModelFactorOvertopping : HydraRingVariable
        {
            public OvertoppingVariableModelFactorOvertopping() : base(12, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN) {}
        }

        private class OvertoppingVariableModelFactorFrunup : HydraRingVariable
        {
            public OvertoppingVariableModelFactorFrunup() : base(120, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 1, 0.07, double.NaN) {}
        }

        private class OvertoppingVariableExponentModelFactorShallow : HydraRingVariable
        {
            public OvertoppingVariableExponentModelFactorShallow() : base(123, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 0.92, 0.24, double.NaN) {}
        }

        #endregion
    }
}