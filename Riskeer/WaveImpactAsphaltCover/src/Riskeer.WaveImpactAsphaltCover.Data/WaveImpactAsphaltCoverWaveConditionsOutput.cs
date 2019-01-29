// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data;

namespace Riskeer.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// Container for the results of a wave impact asphalt cover wave conditions calculation.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsOutput : CloneableObservable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveImpactAsphaltCoverWaveConditionsOutput"/>.
        /// </summary>
        /// <param name="items">The wave conditions output items.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        public WaveImpactAsphaltCoverWaveConditionsOutput(IEnumerable<WaveConditionsOutput> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = items;
        }

        /// <summary>
        /// Gets the output items.
        /// </summary>
        public IEnumerable<WaveConditionsOutput> Items { get; private set; }

        public override object Clone()
        {
            var clone = (WaveImpactAsphaltCoverWaveConditionsOutput) base.Clone();

            clone.Items = Items.Select(s => (WaveConditionsOutput) s.Clone()).ToArray();

            return clone;
        }
    }
}