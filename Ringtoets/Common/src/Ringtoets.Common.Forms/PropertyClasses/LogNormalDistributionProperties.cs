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
    public class LogNormalDistributionProperties : DistributionPropertiesBase<LogNormalDistribution>
    {
        /// <summary>
        /// Creates a new read-only instance of <see cref="LogNormalDistributionProperties"/>.
        /// </summary>
        public LogNormalDistributionProperties() : this(DistributionPropertiesReadOnly.All, null, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="LogNormalDistributionProperties"/>.
        /// </summary>
        /// <param name="propertiesReadOnly">Indicates which properties, if any, should be
        /// marked as read-only.</param>
        /// <param name="observable">The object to be notified of changes to properties.
        /// Can be null if all properties are marked as read-only by <paramref name="propertiesReadOnly"/>.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="observable"/>
        /// is null and any number of properties in this class is editable.</exception>
        public LogNormalDistributionProperties(
            DistributionPropertiesReadOnly propertiesReadOnly, 
            IObservable observable,
            IPropertyChangeHandler handler)
            : base(propertiesReadOnly, observable, handler) {}

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