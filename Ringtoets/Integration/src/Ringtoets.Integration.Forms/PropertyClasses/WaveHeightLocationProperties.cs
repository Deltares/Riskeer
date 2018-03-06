// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryLocationCalculation"/> with <see cref="WaveHeight"/> for properties panel.
    /// </summary>
    public class WaveHeightLocationProperties : HydraulicBoundaryLocationCalculationProperties
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationCalculation">The hydraulic boundary location calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationCalculation"/> is <c>null</c>.</exception>
        public WaveHeightLocationProperties(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
            : base(hydraulicBoundaryLocationCalculation) {}

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Location_WaveHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Location_WaveHeight_Description))]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.Output?.Result ?? RoundedDouble.NaN;
            }
        }

        [PropertyOrder(9)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Result))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.CalculationOutput_Convergence_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Convergence_WaveHeight_Description))]
        public string Convergence
        {
            get
            {
                CalculationConvergence convergence = data.Output?.CalculationConvergence ?? CalculationConvergence.NotCalculated;

                return new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            }
        }
    }
}