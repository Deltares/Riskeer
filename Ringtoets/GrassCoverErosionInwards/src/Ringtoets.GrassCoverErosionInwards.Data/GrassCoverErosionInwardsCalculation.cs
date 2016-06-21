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

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class holds information about a calculation for the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionInwardsCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        public GrassCoverErosionInwardsCalculation()
        {
            Name = Resources.GrassCoverErosionInwardsCalculation_DefaultName;
            InputParameters = new GrassCoverErosionInwardsInput();
            AddDemoInput();
        }

        /// <summary>
        /// Gets the input parameters to perform a grass cover erosion inwards calculation with.
        /// </summary>
        public GrassCoverErosionInwardsInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="ProbabilityAssessmentOutput"/>, which contains the results of a probabilistic calculation.
        /// </summary>
        public ProbabilityAssessmentOutput Output { get; set; }

        public string Comments { get; set; }

        public string Name { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public void ClearOutput()
        {
            Output = null;
        }

        public void ClearHydraulicBoundaryLocation()
        {
            InputParameters.HydraulicBoundaryLocation = null;
        }

        public ICalculationInput GetObservableInput()
        {
            return InputParameters;
        }

        public ICalculationOutput GetObservableOutput()
        {
            return Output;
        }

        private void AddDemoInput()
        {
            InputParameters.DikeProfile = new DikeProfile(new Point2D(0, 0))
            {
                Orientation = (RoundedDouble) 5.5,
                BreakWater = new BreakWater(BreakWaterType.Dam, 10.0),
                DikeGeometry =
                {
                    new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                    new RoughnessPoint(new Point2D(3.3, 4.4), 0.7)
                },
                ForeshoreGeometry =
                {
                    new Point2D(3.3, 4.4),
                    new Point2D(5.5, 6.6)
                },
                CrestLevel = (RoundedDouble) 10
            };
        }
    }
}