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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probabilistics;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all grass cover erosion inwards calculation specific input parameters.
    /// </summary>
    public class GrassCoverErosionInwardsInput : CloneableObservable, ICalculationInputWithHydraulicBoundaryLocation, IUseBreakWater, IUseForeshore
    {
        private const int orientationNumberOfDecimals = 2;

        private static readonly Range<RoundedDouble> orientationValidityRange = new Range<RoundedDouble>(new RoundedDouble(orientationNumberOfDecimals),
                                                                                                         new RoundedDouble(orientationNumberOfDecimals, 360));

        private LogNormalDistribution criticalFlowRate;
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

            SynchronizeDikeProfileInput();

            criticalFlowRate = new LogNormalDistribution(4)
            {
                Mean = (RoundedDouble) 0.004,
                StandardDeviation = (RoundedDouble) 0.0006
            };
            DikeHeightCalculationType = DikeHeightCalculationType.NoCalculation;
            OvertoppingRateCalculationType = OvertoppingRateCalculationType.NoCalculation;
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
                SynchronizeDikeProfileInput();
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
                    throw new ArgumentOutOfRangeException(null, string.Format(RiskeerCommonDataResources.Orientation_Value_needs_to_be_in_Range_0_,
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
        public IEnumerable<RoughnessPoint> DikeGeometry
        {
            get
            {
                return dikeProfile?.DikeGeometry ?? new RoughnessPoint[0];
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
        /// Gets or sets how the dike height should be calculated.
        /// </summary>
        public DikeHeightCalculationType DikeHeightCalculationType { get; set; }

        /// <summary>
        /// Gets or sets how the overtopping rate should be calculated.
        /// </summary>
        public OvertoppingRateCalculationType OvertoppingRateCalculationType { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Dike Height.
        /// </summary>
        public bool ShouldDikeHeightIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Overtopping Flow.
        /// </summary>
        public bool ShouldOvertoppingRateIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets or sets if the illustration points should be calculated for Overtopping Output.
        /// </summary>
        public bool ShouldOvertoppingOutputIllustrationPointsBeCalculated { get; set; }

        /// <summary>
        /// Gets the value <c>true</c> if the parameters of the instance of
        /// <see cref="GrassCoverErosionInwardsInput"/> that are derived from
        /// <see cref="DikeProfile"/> match the properties of
        /// <see cref="DikeProfile"/>; or <c>false</c> if this is not the case,
        /// or if there is no <see cref="DikeProfile"/> assigned.
        /// </summary>
        public bool IsDikeProfileInputSynchronized
        {
            get
            {
                if (dikeProfile == null)
                {
                    return false;
                }

                BreakWater breakwater = dikeProfile.HasBreakWater
                                            ? new BreakWater(dikeProfile.BreakWater.Type, dikeProfile.BreakWater.Height)
                                            : GetDefaultBreakWater();

                return Equals(UseBreakWater, dikeProfile.HasBreakWater)
                       && Equals(BreakWater, breakwater)
                       && Equals(Orientation, dikeProfile.Orientation)
                       && Equals(DikeHeight, dikeProfile.DikeHeight)
                       && UseForeshore == dikeProfile.ForeshoreGeometry.Count() > 1;
            }
        }

        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

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

        /// <summary>
        /// Applies the properties of the <see cref="DikeProfile"/> to the
        /// parameters of the instance of <see cref="GrassCoverErosionInwardsInput"/>.
        /// </summary>
        /// <remarks>When no dike profile is present, the input parameters are set to default values.</remarks>
        public void SynchronizeDikeProfileInput()
        {
            if (dikeProfile != null)
            {
                Orientation = dikeProfile.Orientation;
                UseForeshore = dikeProfile.ForeshoreGeometry.Count() > 1;
                UseBreakWater = dikeProfile.HasBreakWater;
                BreakWater = dikeProfile.HasBreakWater
                                 ? new BreakWater(dikeProfile.BreakWater.Type, dikeProfile.BreakWater.Height)
                                 : GetDefaultBreakWater();
                DikeHeight = dikeProfile.DikeHeight;
            }
            else
            {
                SetDefaultDikeProfileProperties();
            }
        }

        public override object Clone()
        {
            var clone = (GrassCoverErosionInwardsInput) base.Clone();

            clone.criticalFlowRate = (LogNormalDistribution) CriticalFlowRate.Clone();
            clone.BreakWater = (BreakWater) BreakWater.Clone();

            return clone;
        }

        private void SetDefaultDikeProfileProperties()
        {
            Orientation = RoundedDouble.NaN;
            UseForeshore = false;
            UseBreakWater = false;
            BreakWater = GetDefaultBreakWater();
            DikeHeight = (RoundedDouble) double.NaN;
        }

        private static BreakWater GetDefaultBreakWater()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }
    }
}