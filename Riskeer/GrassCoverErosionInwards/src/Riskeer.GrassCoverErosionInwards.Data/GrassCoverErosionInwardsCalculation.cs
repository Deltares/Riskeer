// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Base;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class holds information about a calculation for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsCalculation : CloneableObservable, ICalculation<GrassCoverErosionInwardsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public GrassCoverErosionInwardsCalculation()
        {
            Name = RiskeerCommonDataResources.Calculation_DefaultName;
            InputParameters = new GrassCoverErosionInwardsInput();
            Comments = new Comment();
        }

        /// <summary>
        /// Gets or sets <see cref="GrassCoverErosionInwardsOutput"/>, which contains the results of a probabilistic calculation.
        /// </summary>
        public GrassCoverErosionInwardsOutput Output { get; set; }

        /// <summary>
        /// Gets the input parameters to perform a grass cover erosion inwards calculation with.
        /// </summary>
        public GrassCoverErosionInwardsInput InputParameters { get; private set; }

        public string Name { get; set; }

        public bool ShouldCalculate
        {
            get
            {
                return ShouldCalculateOvertopping()
                       || ShouldCalculateDikeHeight()
                       || ShouldCalculateOvertoppingRate();
            }
        }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public Comment Comments { get; private set; }

        /// <summary>
        /// Clears the calculated illustration points.
        /// </summary>
        public void ClearIllustrationPoints()
        {
            Output?.ClearIllustrationPoints();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The name of this calculation.</returns>
        public override string ToString()
        {
            return Name;
        }

        public override object Clone()
        {
            var clone = (GrassCoverErosionInwardsCalculation) base.Clone();

            clone.Comments = (Comment) Comments.Clone();
            clone.InputParameters = (GrassCoverErosionInwardsInput) InputParameters.Clone();

            if (Output != null)
            {
                clone.Output = (GrassCoverErosionInwardsOutput) Output.Clone();
            }

            return clone;
        }

        public void ClearOutput()
        {
            Output = null;
        }

        private bool ShouldCalculateOvertopping()
        {
            return !HasOutput || Output.OvertoppingOutput.HasGeneralResult != InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated;
        }

        private bool ShouldCalculateDikeHeight()
        {
            return Output.DikeHeightOutput != null && Output.DikeHeightOutput.HasGeneralResult != InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated;
        }

        private bool ShouldCalculateOvertoppingRate()
        {
            return Output.OvertoppingRateOutput != null && Output.OvertoppingRateOutput.HasGeneralResult != InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated;
        }
    }
}