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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all grass cover erosion inwards calculation specific input parameters.
    /// </summary>
    public class GrassCoverErosionInwardsInput : Observable, ICalculationInput, IUseBreakWater,
                                                 IUseForeshore
    {
        private const int orientationNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> orientationValidityRange = new Range<RoundedDouble>(new RoundedDouble(orientationNumberOfDecimals),
                                                                                                         new RoundedDouble(orientationNumberOfDecimals, 360));

        private readonly LogNormalDistribution criticalFlowRate;
        private RoundedDouble orientation;
        private RoundedDouble dikeHeight;
        private DikeProfile dikeProfile;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInput"/>.
        /// </summary>
        public GrassCoverErosionInwardsInput()
        {
            orientation = new RoundedDouble(orientationNumberOfDecimals);
            dikeHeight = new RoundedDouble(2);

            UpdateProfileParameters();

            criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = (RoundedDouble) 0.004,
                StandardDeviation = (RoundedDouble) 0.0006
            };
            DikeHeightCalculationType = DikeHeightCalculationType.NoCalculation;
        }

        /// <summary>
        /// Gets or sets the dike profile.
        /// </summary>
        public DikeProfile DikeProfile
        {
            get
            {
                return dikeProfile;
            }
            set
            {
                dikeProfile = value;
                UpdateProfileParameters();
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the dike profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                RoundedDouble newOrientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
                if (!double.IsNaN(newOrientation) && !orientationValidityRange.InRange(newOrientation))
                {
                    throw new ArgumentOutOfRangeException(null, string.Format(RingtoetsCommonDataResources.Orientation_Value_needs_to_be_in_Range_0_,
                                                                                       orientationValidityRange));
                }
                orientation = newOrientation;
            }
        }

        /// <summary>
        /// Gets the geometry of the dike with roughness data.
        /// </summary>
        /// <remarks>
        /// The roughness of a <see cref="RoughnessPoint"/> in the array represents
        /// the roughness of the section between this <see cref="RoughnessPoint"/>
        /// and the succeeding <see cref="RoughnessPoint"/>. The roughness of the last
        /// point is irrelevant.
        /// </remarks>
        public RoughnessPoint[] DikeGeometry
        {
            get
            {
                return dikeProfile != null
                           ? dikeProfile.DikeGeometry.ToArray()
                           : new RoughnessPoint[0];
            }
        }

        /// <summary>
        /// Gets or sets the height of the dike [m+NAP].
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
        /// Gets or sets the critical flow rate.
        /// </summary>
        public LogNormalDistribution CriticalFlowRate
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
        /// Gets or sets the hydraulic boundary location.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets how the dike height should be calculated.
        /// </summary>
        public DikeHeightCalculationType DikeHeightCalculationType { get; set; }

        public bool UseBreakWater { get; set; }

        public BreakWater BreakWater { get; private set; }

        public bool UseForeshore { get; set; }

        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return dikeProfile != null
                           ? dikeProfile.ForeshoreGeometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
            }
        }

        private void UpdateProfileParameters()
        {
            if (dikeProfile == null)
            {
                Orientation = RoundedDouble.NaN;
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWater();
                DikeHeight = (RoundedDouble) double.NaN;
            }
            else
            {
                Orientation = dikeProfile.Orientation;
                UseForeshore = dikeProfile.ForeshoreGeometry.Count() > 1;
                UseBreakWater = dikeProfile.HasBreakWater;
                BreakWater = dikeProfile.HasBreakWater
                                 ? new BreakWater(dikeProfile.BreakWater.Type, dikeProfile.BreakWater.Height)
                                 : GetDefaultBreakWater();
                DikeHeight = dikeProfile.DikeHeight;
            }
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}