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
using Ringtoets.Common.Data;
using Ringtoets.Common.Placeholder;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds the information which can be made visible in the graphical interface of the application.
    /// </summary>
    public class PipingCalculation : Observable, ICalculationItem, IPipingCalculationItem
    {
        /// <summary>
        /// Constructs a new instance of <see cref="PipingCalculation"/> with default values set for some of the parameters.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public PipingCalculation(GeneralPipingInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }
            Name = Resources.PipingCalculation_DefaultName;

            Comments = new InputPlaceholder(Resources.Comments_DisplayName);
            InputParameters = new PipingInput(generalInputParameters);
        }

        /// <summary>
        /// Gets the user notes for this calculation.
        /// </summary>
        public PlaceholderWithReadonlyName Comments { get; private set; }

        /// <summary>
        /// Gets the input parameters to perform a piping calculation with.
        /// </summary>
        public PipingInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a Piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

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

        public void ClearOutput()
        {
            Output = null;
        }
    }
}