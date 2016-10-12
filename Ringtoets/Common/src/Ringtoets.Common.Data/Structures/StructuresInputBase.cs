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

using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.Structures
{
    /// <summary>
    /// Base class that holds all structures calculation specific input parameters.
    /// </summary>
    public abstract class StructuresInputBase<T> : Observable, ICalculationInput, IUseBreakWater, IUseForeshore
        where T : StructureBase
    {
        private T structure;
        private ForeshoreProfile foreshoreProfile;

        protected StructuresInputBase()
        {
            UpdateForeshoreProperties();
        }

        #region Foreshore Profile

        /// <summary>
        /// Gets or sets the foreshore profile.
        /// </summary>
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return foreshoreProfile;
            }
            set
            {
                foreshoreProfile = value;
                UpdateForeshoreProperties();
            }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="BreakWater"/> needs to be taken into account.
        /// </summary>
        public bool UseBreakWater { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="ForeshoreProfile"/> needs to be taken into account.
        /// </summary>
        public bool UseForeshore { get; set; }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return foreshoreProfile != null
                           ? foreshoreProfile.Geometry
                           : new RoundedPoint2DCollection(2, Enumerable.Empty<Point2D>());
            }
        }

        /// <summary>
        /// Gets the <see cref="BreakWater"/>.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        private void UpdateForeshoreProperties()
        {
            if (foreshoreProfile == null)
            {
                UseForeshore = false;
                UseBreakWater = false;
                BreakWater = GetDefaultBreakWaterProperties();
            }
            else
            {
                UseForeshore = foreshoreProfile.Geometry.Count() > 1;
                UseBreakWater = foreshoreProfile.HasBreakWater;
                BreakWater = foreshoreProfile.HasBreakWater ?
                                 new BreakWater(foreshoreProfile.BreakWater.Type, foreshoreProfile.BreakWater.Height) :
                                 GetDefaultBreakWaterProperties();
            }
        }

        private static BreakWater GetDefaultBreakWaterProperties()
        {
            return new BreakWater(BreakWaterType.Dam, 0.0);
        }

        #endregion
    }
}