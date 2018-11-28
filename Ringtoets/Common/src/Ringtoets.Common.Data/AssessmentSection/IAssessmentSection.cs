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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.AssessmentSection
{
    /// <summary>
    /// Base implementation of assessment sections.
    /// </summary>
    public interface IAssessmentSection : IObservable
    {
        /// <summary>
        /// Gets the identifier of the assessment section.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets or sets the name of the assessment section.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the comments associated with the assessment section.
        /// </summary>
        Comment Comments { get; }

        /// <summary>
        /// Gets the composition of the assessment section, i.e. what type of elements can 
        /// be found within the assessment section.
        /// </summary>
        AssessmentSectionComposition Composition { get; }

        /// <summary>
        /// Gets the reference line defining the geometry of the assessment section.
        /// </summary>
        ReferenceLine ReferenceLine { get; }

        /// <summary>
        /// Gets or sets the contribution of each failure mechanism available in this assessment section.
        /// </summary>
        FailureMechanismContribution FailureMechanismContribution { get; }

        /// <summary>
        /// Gets the hydraulic boundary database.
        /// </summary>
        HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }

        /// <summary>
        /// Gets the data that represents the background for all geo-referenced data.
        /// </summary>
        BackgroundData BackgroundData { get; }

        /// <summary>
        /// Gets the water level calculations corresponding to the factorized signaling norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the water level calculations corresponding to the signaling norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalingNorm { get; }

        /// <summary>
        /// Gets the water level calculations corresponding to the lower limit norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the water level calculations corresponding to the factorized lower limit norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the wave height calculations corresponding to the factorized signaling norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedSignalingNorm { get; }

        /// <summary>
        /// Gets the wave height calculations corresponding to the signaling norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForSignalingNorm { get; }

        /// <summary>
        /// Gets the wave height calculations corresponding to the lower limit norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForLowerLimitNorm { get; }

        /// <summary>
        /// Gets the wave height calculations corresponding to the factorized lower limit norm.
        /// </summary>
        IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedLowerLimitNorm { get; }

        /// <summary>
        /// Gets the failure mechanisms corresponding to the assessment section.
        /// </summary>
        IEnumerable<IFailureMechanism> GetFailureMechanisms();

        /// <summary>
        /// Gets the contributing failure mechanisms of the assessment section.
        /// </summary>
        /// <returns>A collection of failure mechanisms.</returns>
        IEnumerable<IFailureMechanism> GetContributingFailureMechanisms();

        /// <summary>
        /// Changes <see cref="Composition"/> and reconfigures <see cref="FailureMechanismContribution"/>
        /// and the failure mechanisms returned by <see cref="GetFailureMechanisms"/>.
        /// </summary>
        /// <param name="newComposition">The new composition description.</param>
        void ChangeComposition(AssessmentSectionComposition newComposition);
    }
}