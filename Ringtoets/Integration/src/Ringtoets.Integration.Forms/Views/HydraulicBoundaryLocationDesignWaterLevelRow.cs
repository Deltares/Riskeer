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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation"/> with <see cref="DesignWaterLevel"/>.
    /// </summary>
    internal class HydraulicBoundaryLocationDesignWaterLevelRow
    {
        private readonly DesignWaterLevelLocationContext designWaterLevelLocationContext;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationDesignWaterLevelRow"/>.
        /// </summary>
        /// <param name="designWaterLevelLocationContext">The <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designWaterLevelLocationContext"/> is <c>null</c>.</exception>
        internal HydraulicBoundaryLocationDesignWaterLevelRow(DesignWaterLevelLocationContext designWaterLevelLocationContext)
        {
            if (designWaterLevelLocationContext == null)
            {
                throw new ArgumentNullException("designWaterLevelLocationContext");
            }

            this.designWaterLevelLocationContext = designWaterLevelLocationContext;
        }

        /// <summary>
        /// Gets whether the <see cref="HydraulicBoundaryLocationDesignWaterLevelRow"/> is set to be calculated.
        /// </summary>
        public bool ToCalculate { get; set; }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return designWaterLevelLocationContext.HydraulicBoundaryLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return designWaterLevelLocationContext.HydraulicBoundaryLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return designWaterLevelLocationContext.HydraulicBoundaryLocation.Location;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.DesignWaterLevel"/>.
        /// </summary>
        [TypeConverter(typeof(FailureMechanismSectionResultNoValueRoundedDoubleConverter))]
        public RoundedDouble DesignWaterLevel
        {
            get
            {
                return new RoundedDouble(2, designWaterLevelLocationContext.HydraulicBoundaryLocation.DesignWaterLevel);
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation"/>.
        /// </summary>
        public DesignWaterLevelLocationContext DesignWaterLevelLocationContext
        {
            get
            {
                return designWaterLevelLocationContext;
            }
        }
    }
}