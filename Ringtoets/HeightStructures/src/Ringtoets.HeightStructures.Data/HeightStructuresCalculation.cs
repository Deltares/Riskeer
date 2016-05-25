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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.HeightStructures.Data.Properties;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// This class holds information about a calculation for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructuresCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculation"/>.
        /// </summary>
        /// <param name="generalInputParameters">General height structures calculation input parameters that apply to each calculation.</param>
        /// <param name="normProbabilityInput">General norm probabilistic parameters that apply to each 
        /// calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <list type="bullet">
        /// <item><paramref name="generalInputParameters"/> is <c>null</c>.</item>
        /// <item><paramref name="normProbabilityInput"/> is <c>null</c>.</item>
        /// </list> </exception>
        public HeightStructuresCalculation(GeneralHeightStructuresInput generalInputParameters, NormProbabilityInput normProbabilityInput)
        {
            if (normProbabilityInput == null)
            {
                throw new ArgumentNullException("normProbabilityInput");
            }
            InputParameters = new HeightStructuresInput(generalInputParameters);
            NormProbabilityInput = normProbabilityInput;
            Name = Resources.HeightStructuresCalculation_DefaultName;
            AddDemoInput();
        }

        /// <summary>
        /// Gets the input parameters to perform a height structures calculation with.
        /// </summary>
        public HeightStructuresInput InputParameters { get; private set; }

        /// <summary>
        /// Gets the length-effect parameters.
        /// </summary>
        public NormProbabilityInput NormProbabilityInput { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="ProbabilisticOutput"/>, which contains the results of a height structures calculation.
        /// </summary>
        public ProbabilisticOutput Output { get; set; }

        public string Name { get; set; }

        public string Comments { get; set; }

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

        public void ClearHydraulicBoundaryLocation() {}

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
            InputParameters.LevelOfCrestOfStructure.Mean = (RoundedDouble) 3.5;
            InputParameters.OrientationOfTheNormalOfTheStructure = (RoundedDouble) 115;
            InputParameters.AllowableIncreaseOfLevelForStorage.Mean = (RoundedDouble) 1.0;
            InputParameters.StorageStructureArea.Mean = (RoundedDouble) 1000000;
            InputParameters.FlowWidthAtBottomProtection.Mean = (RoundedDouble) 18;
            InputParameters.CriticalOvertoppingDischarge.Mean = (RoundedDouble) 1;
            InputParameters.WidthOfFlowApertures.Mean = (RoundedDouble) 18;
            InputParameters.DeviationOfTheWaveDirection = (RoundedDouble) 0;
            InputParameters.FailureProbabilityOfStructureGivenErosion = (RoundedDouble) 1;
        }
    }
}