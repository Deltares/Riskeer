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

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// An <see cref="ObjectProperties{T}"/> implementation for <see cref="LogNormalDistribution"/>
    /// properties.
    /// </summary>
    public class LogNormalDistributionProperties : DistributionProperties
    {
        /// <summary>
        /// Creates a new read-only instance of <see cref="LogNormalDistributionProperties"/>.
        /// </summary>
        public LogNormalDistributionProperties() : this(null, DistributionPropertiesReadOnly.All) {}

        /// <summary>
        /// Creates a new instance of <see cref="LogNormalDistributionProperties"/>.
        /// </summary>
        /// <param name="observerable">Object to observe to notify upon change.</param>
        /// <param name="propertiesReadOnly">Sets if <see cref="DistributionProperties.Mean"/> and/or 
        /// <see cref="DistributionProperties.StandardDeviation"/> should be marked read-only.</param>
        public LogNormalDistributionProperties(IObservable observerable, DistributionPropertiesReadOnly propertiesReadOnly)
            : base(propertiesReadOnly)
        {
            Observerable = observerable;
        }

        public override string DistributionType
        {
            get
            {
                return Resources.DistributionType_LogNormal;
            }
        }

        [ResourcesDescription(typeof(Resources), "LogNormalDistribution_Mean_Description")]
        public override RoundedDouble Mean
        {
            get
            {
                return base.Mean;
            }
            set
            {
                base.Mean = value;
            }
        }

        [ResourcesDescription(typeof(Resources), "LogNormalDistribution_StandardDeviation_Description")]
        public override RoundedDouble StandardDeviation
        {
            get
            {
                return base.StandardDeviation;
            }
            set
            {
                base.StandardDeviation = value;
            }
        }
    }
}