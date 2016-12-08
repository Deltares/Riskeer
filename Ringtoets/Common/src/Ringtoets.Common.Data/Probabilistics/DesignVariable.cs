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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probabilistics
{
    public abstract class DesignVariable<TDistributionType> where TDistributionType : IDistribution {
        private TDistributionType distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignVariable{T}"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="Distribution"/> is 
        /// <c>null</c>.</exception>
        protected DesignVariable(TDistributionType distribution)
        {
            Distribution = distribution;
        }

        /// <summary>
        /// Gets or sets the probabilistic distribution of the parameter being modeled.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is 
        /// <c>null</c>.</exception>
        public TDistributionType Distribution
        {
            get
            {
                return distribution;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.DesignVariable_GetDesignValue_Distribution_must_be_set);
                }
                distribution = value;
            }
        }

        /// <summary>
        /// Gets a projected (design) value based on the <see cref="Distribution"/>.
        /// </summary>
        /// <returns>A design value.</returns>
        public abstract RoundedDouble GetDesignValue();
    }
}