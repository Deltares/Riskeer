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

using System;
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
        /// <param name="generalInputParameters">General grass cover erosion inwards calculation input parameters 
        /// that apply to each calculation.</param>
        /// <param name="probabilityAssessmentInput">General probabilistic assessment parameters that apply to each 
        /// calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <list type="bullet">
        /// <item><paramref name="generalInputParameters"/> is <c>null</c>.</item>
        /// <item><paramref name="probabilityAssessmentInput"/> is <c>null</c>.</item>
        /// </list> </exception>
        public GrassCoverErosionInwardsCalculation(GeneralGrassCoverErosionInwardsInput generalInputParameters,
                                                   ProbabilityAssessmentInput probabilityAssessmentInput)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException("probabilityAssessmentInput");
            }
            Name = Resources.GrassCoverErosionInwardsCalculation_DefaultName;
            InputParameters = new GrassCoverErosionInwardsInput(generalInputParameters);
            ProbabilityAssessmentInput = probabilityAssessmentInput;
            AddDemoInput();
        }

        /// <summary>
        /// Gets the input parameters to perform a grass cover erosion inwards calculation with.
        /// </summary>
        public GrassCoverErosionInwardsInput InputParameters { get; private set; }

        /// <summary>
        /// Gets the probability assessment input.
        /// </summary>
        public ProbabilityAssessmentInput ProbabilityAssessmentInput { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="ProbabilisticOutput"/>, which contains the results of a probabilistic calculation.
        /// </summary>
        public ProbabilisticOutput Output { get; set; }

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
            // BreakWater
            InputParameters.BreakWater.Type = BreakWaterType.Dam;
            InputParameters.BreakWater.Height = (RoundedDouble) 10;
            InputParameters.UseBreakWater = true;

            // Orientation
            InputParameters.Orientation = (RoundedDouble) 5.5;

            // Dike and Foreshore
            var dikeSections = new[]
            {
                new RoughnessProfileSection(new Point2D(1.1, 2.2), new Point2D(3.3, 4.4), 1.1)
            };
            var foreshoreSections = new[]
            {
                new ProfileSection(new Point2D(3.3, 4.4), new Point2D(5.5, 6.6))
            };
            InputParameters.SetDikeGeometry(dikeSections);
            InputParameters.SetForeshoreGeometry(foreshoreSections);
            InputParameters.UseForeshore = true;

            // Dike height
            InputParameters.DikeHeight = (RoundedDouble) 10;
        }
    }
}