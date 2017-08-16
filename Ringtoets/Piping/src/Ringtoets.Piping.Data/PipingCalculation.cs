﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Calculation;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds information about a calculation for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingCalculation : Observable, ICalculation, ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculation"/> with default values set for some of the parameters.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that
        /// are the same across all piping calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/> is <c>null</c>.</exception>
        public PipingCalculation(GeneralPipingInput generalInputParameters)
        {
            Name = RingtoetsCommonDataResources.Calculation_DefaultName;
            InputParameters = new PipingInput(generalInputParameters);
            Comments = new Comment();
        }

        /// <summary>
        /// Gets the input parameters to perform a piping calculation with.
        /// </summary>
        public PipingInput InputParameters { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="PipingOutput"/>, which contains the results of a piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

        /// <summary>
        /// Gets or sets the semi-probabilistic calculation result.
        /// </summary>
        public PipingSemiProbabilisticOutput SemiProbabilisticOutput { get; set; }

        public string Name { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null || SemiProbabilisticOutput != null;
            }
        }

        public Comment Comments { get; private set; }

        public void ClearOutput()
        {
            Output = null;
            SemiProbabilisticOutput = null;
        }

        public object Clone()
        {
            var clone = (PipingCalculation) MemberwiseClone();

            clone.Comments = (Comment) Comments.Clone();
            clone.InputParameters = (PipingInput) InputParameters.Clone();

            if (Output != null)
            {
                clone.Output = (PipingOutput) Output.Clone();
            }

            if (SemiProbabilisticOutput != null)
            {
                clone.SemiProbabilisticOutput = (PipingSemiProbabilisticOutput) SemiProbabilisticOutput.Clone();
            }

            return clone;
        }
    }
}