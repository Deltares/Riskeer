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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// A read-only <see cref="ObjectProperties{T}"/> implementation for <see cref="NormalDistribution"/>
    /// properties.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ReadOnlyNormalDistributionProperties : DistributionProperties
    {
        public override string DistributionType
        {
            get
            {
                return "Normale verdeling";
            }
        }

        [ReadOnly(true)]
        public override RoundedDouble Mean
        {
            get
            {
                return base.Mean;
            }
        }

        [ReadOnly(true)]
        public override RoundedDouble StandardDeviation
        {
            get
            {
                return base.StandardDeviation;
            }
        }
    }
}