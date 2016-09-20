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

using Core.Common.Base.Data;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// Property control the <see cref="GrassCoverErosionOutwardsWaveConditionsInputContext"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsInputContextProperties
        : WaveConditionsInputContextProperties<GrassCoverErosionOutwardsWaveConditionsInputContext>
    {
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionOutwardsHydraulicBoundaryLocation_DesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionOutwardsWaveConditionsInputContextProperties_DesignWaterLevel_Description")]
        public override RoundedDouble AssessmentLevel
        {
            get
            {
                return base.AssessmentLevel;
            }
        }

        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionOutwardsWaveConditionsInputContextProperties_UpperBoundaryDesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionOutwardsWaveConditionsInputContextProperties_UpperBoundaryDesignWaterLevel_Description")]
        public override RoundedDouble UpperBoundaryDesignWaterLevel
        {
            get
            {
                return base.UpperBoundaryDesignWaterLevel;
            }
        }

        public override WaveConditionsRevetment RevetmentType
        {
            get
            {
                return WaveConditionsRevetment.Grass;
            }
        }
    }
}