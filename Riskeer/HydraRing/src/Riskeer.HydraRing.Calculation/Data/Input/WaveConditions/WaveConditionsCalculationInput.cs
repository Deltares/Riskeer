﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Util;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Data.Input.WaveConditions
{
    /// <summary>
    /// Container of all data necessary for performing a wave conditions calculation (Q-variant) via Hydra-Ring.
    /// </summary>
    public abstract class WaveConditionsCalculationInput : HydraRingCalculationInput
    {
        private readonly double waterLevel;
        private readonly double a;
        private readonly double b;

        /// <summary>
        /// Creates a new instance of the <see cref="WaveConditionsCalculationInput"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section.</param>
        /// <param name="sectionNormal">The normal of the section.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="targetProbability">The target probability.</param>
        /// <param name="forelandPoints">The foreland points.</param>
        /// <param name="breakWater">The break water.</param>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value.</param>
        /// <param name="b">The b-value.</param>
        /// <remarks>As a part of the constructor, the <paramref name="targetProbability"/> is automatically converted into a reliability index.</remarks>
        protected WaveConditionsCalculationInput(int sectionId,
                                                 double sectionNormal,
                                                 long hydraulicBoundaryLocationId,
                                                 double targetProbability,
                                                 IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                 HydraRingBreakWater breakWater,
                                                 double waterLevel,
                                                 double a,
                                                 double b)
            : base(hydraulicBoundaryLocationId)
        {
            Section = new HydraRingSection(sectionId, double.NaN, sectionNormal);
            Beta = StatisticsConverter.ProbabilityToReliability(targetProbability);
            ForelandPoints = forelandPoints;
            BreakWater = breakWater;

            this.waterLevel = waterLevel;
            this.a = a;
            this.b = b;
        }

        public override HydraRingFailureMechanismType FailureMechanismType => HydraRingFailureMechanismType.QVariant;

        public override int CalculationTypeId => 8;

        public override int VariableId => 114;

        public override int FaultTreeModelId => 6;

        public override HydraRingSection Section { get; }

        public override IEnumerable<HydraRingVariable> Variables
        {
            get
            {
                yield return new DeterministicHydraRingVariable(113, waterLevel);
                yield return new DeterministicHydraRingVariable(114, 1.0);
                yield return new DeterministicHydraRingVariable(115, a);
                yield return new DeterministicHydraRingVariable(116, b);
            }
        }

        public override IEnumerable<HydraRingForelandPoint> ForelandPoints { get; }

        public override HydraRingBreakWater BreakWater { get; }

        public override double Beta { get; }
    }
}