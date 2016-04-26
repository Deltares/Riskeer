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

using System;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information which can be made visible in the graphical interface of the application.
    /// </summary>
    public class PipingCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Constructs a new instance of <see cref="PipingCalculation"/> with default values set for some of the parameters.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <param name="normProbabilityInputParameters">General semi-probabilistic parameters that 
        /// are used in a semi-probabilistic piping assessment.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/> or 
        /// <paramref name="normProbabilityInputParameters"/> is <c>null</c>.</exception>
        public PipingCalculation(GeneralPipingInput generalInputParameters, NormProbabilityPipingInput normProbabilityInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }
            if (normProbabilityInputParameters == null)
            {
                throw new ArgumentNullException("normProbabilityInputParameters");
            }
            Name = Resources.PipingCalculation_DefaultName;

            InputParameters = new PipingInput(generalInputParameters);
            NormProbabilityParameters = normProbabilityInputParameters;
        }

        /// <summary>
        /// Gets the parameters required to perform a semi-probabilistic assessment.
        /// </summary>
        public NormProbabilityPipingInput NormProbabilityParameters { get; private set; }

        /// <summary>
        /// Gets the input parameters to perform a piping calculation with.
        /// </summary>
        public PipingInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a Piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

        /// <summary>
        /// Gets or sets the semi-probabilistic calculation result.
        /// </summary>
        public PipingSemiProbabilisticOutput SemiProbabilisticOutput { get; set; }

        public ICalculationInput Input
        {
            get
            {
                return InputParameters;
            }
        }

        /// <summary>
        /// Gets or sets the name of this calculation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the <see cref="PipingCalculation"/> has <see cref="Output"/>.
        /// </summary>
        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public string Comments { get; set; }

        public void ClearOutput()
        {
            Output = null;
        }

        public void ClearHydraulicBoundaryLocation()
        {
            InputParameters.HydraulicBoundaryLocation = null;
        }
    }
}