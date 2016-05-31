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
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// An <see cref="ObjectProperties{T}"/> implementation for <see cref="NormalDistribution"/>
    /// properties.
    /// </summary>
    public class NormalDistributionProperties : DistributionProperties
    {
        /// <summary>
        /// Creates a new read-only instance of <see cref="NormalDistributionProperties"/>.
        /// </summary>
        public NormalDistributionProperties() : this(null, DistributionPropertiesReadOnly.All) {}

        /// <summary>
        /// Creates a new instance of <see cref="NormalDistributionProperties"/>.
        /// </summary>
        /// <param name="observerable">Object to observe to notify upon change.</param>
        /// <param name="propertiesReadOnly">Sets if <see cref="DistributionProperties.Mean"/> and/or 
        /// <see cref="DistributionProperties.StandardDeviation"/> should be marked read-only.</param>
        public NormalDistributionProperties(IObservable observerable, DistributionPropertiesReadOnly propertiesReadOnly) : base(propertiesReadOnly)
        {
            Observerable = observerable;
            
        }

        public override string DistributionType
        {
            get
            {
                return Resources.DistributionType_Normal;
            }
        }
    }
}