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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all grass cover erosion inwards calculation specific input parameters.
    /// </summary>
    public class GrassCoverErosionInwardsInput : Observable, ICalculationInput
    {
        private readonly LognormalDistribution criticalFlowRate;
        private readonly GeneralGrassCoverErosionInwardsInput generalInputParameters;
        private RoundedDouble orientation;
        private RoundedDouble dikeHeight;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInput"/>.
        /// </summary>
        /// <param name="generalInputParameters">General grass cover erosion inwards calculation input parameters that apply to each calculation.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="generalInputParameters"/> is <c>null</c>.</exception>
        public GrassCoverErosionInwardsInput(GeneralGrassCoverErosionInwardsInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException("generalInputParameters");
            }
            this.generalInputParameters = generalInputParameters;

            orientation = new RoundedDouble(2);
            dikeHeight = new RoundedDouble(2);
            BreakWater = new BreakWater(BreakWaterType.Caisson, 0);
            criticalFlowRate = new LognormalDistribution(2);
            DikeGeometry = Enumerable.Empty<RoughnessProfileSection>();
            ForeshoreGeometry = Enumerable.Empty<RoughnessProfileSection>();
        }

        /// <summary>
        /// Gets the dike's geometry (without foreshore geometry).
        /// </summary>
        public IEnumerable<RoughnessProfileSection> DikeGeometry { get; private set; }

        /// <summary>
        /// Gets the dike's foreshore geometry.
        /// </summary>
        public IEnumerable<RoughnessProfileSection> ForeshoreGeometry { get; private set; }

        /// <summary>
        /// Gets or sets the dike's orientation
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets the dike's critical flow rate.
        /// </summary>
        public LognormalDistribution CriticalFlowRate
        {
            get
            {
                return criticalFlowRate;
            }
            set
            {
                criticalFlowRate.Mean = value.Mean;
                criticalFlowRate.StandardDeviation = value.StandardDeviation;
            }
        }

        /// <summary>
        /// Gets or sets if <see cref="ForeshoreGeometry"/> needs to be taken into account.
        /// </summary>
        /// <remarks>Value of <see cref="ForeshoreGeometry"/> must not be reset when <see cref="UseForeshore"/> is set to <c>false</c>.</remarks>
        public bool UseForeshore { get; set; }

        /// <summary>
        /// Gets or sets the dike height.
        /// </summary>
        public RoundedDouble DikeHeight
        {
            get
            {
                return dikeHeight;
            }
            set
            {
                dikeHeight = value.ToPrecision(dikeHeight.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Gets or sets if <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool UseBreakWater { get; set; }

        /// <summary>
        /// Gets the <see cref="BreakWater"/>.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        /// <summary>
        /// Gets or set the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Sets the grass cover erosion inwards dike geometry.
        /// </summary>
        /// <param name="profileSections">The grass cover erosion inwards geometry points.</param>
        public void SetDikeGeometry(IEnumerable<RoughnessProfileSection> profileSections)
        {
            if (profileSections == null)
            {
                throw new ArgumentNullException("profileSections");
            }
            DikeGeometry = profileSections;
        }

        /// <summary>
        /// Sets the grass cover erosion inwards foreshore geometry.
        /// </summary>
        /// <param name="profileSections">The grass cover erosion inwards geometry points.</param>
        public void SetForeshoreGeometry(IEnumerable<RoughnessProfileSection> profileSections)
        {
            if (profileSections == null)
            {
                throw new ArgumentNullException("profileSections");
            }
            ForeshoreGeometry = profileSections;
        }

        #region General input parameters

        /// <summary>
        /// Gets the model factor critical overtopping.
        /// </summary>
        public double CriticalOvertoppingModelFactor
        {
            get
            {
                return generalInputParameters.CriticalOvertoppingModelFactor;
            }
        }

        /// <summary>
        /// Gets the factor fb variable.
        /// </summary>
        public NormalDistribution FbFactor
        {
            get
            {
                return generalInputParameters.FbFactor;
            }
        }

        /// <summary>
        /// Gets the factor fn variable.
        /// </summary>
        public NormalDistribution FnFactor
        {
            get
            {
                return generalInputParameters.FnFactor;
            }
        }

        /// <summary>
        /// Gets the model factor overtopping.
        /// </summary>
        public double OvertoppingModelFactor
        {
            get
            {
                return generalInputParameters.OvertoppingModelFactor;
            }
        }

        /// <summary>
        /// Gets the factor mz2 (or frunup) variable.
        /// </summary>
        public NormalDistribution FrunupModelFactor
        {
            get
            {
                return generalInputParameters.FrunupModelFactor;
            }
        }

        /// <summary>
        /// Gets the factor fshallow variable.
        /// </summary>
        public NormalDistribution FshallowModelFactor
        {
            get
            {
                return generalInputParameters.FshallowModelFactor;
            }
        }

        #endregion
    }
}