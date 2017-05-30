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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object which wraps a <see cref="MacroStabilityInwardsOutput"/> and a <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>.
    /// </summary>
    public class MacroStabilityInwardsOutputContext : WrappedObjectContextBase<MacroStabilityInwardsOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutputContext"/>.
        /// </summary>
        /// <param name="macroStabilityInwardsOutput">The <see cref="MacroStabilityInwardsOutput"/> object to wrap.</param>
        /// <param name="semiProbabilisticOutput">The <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>
        /// created from <paramref name="macroStabilityInwardsOutput"/>.</param>
        public MacroStabilityInwardsOutputContext(MacroStabilityInwardsOutput macroStabilityInwardsOutput,
                                                  MacroStabilityInwardsSemiProbabilisticOutput semiProbabilisticOutput)
            : base(macroStabilityInwardsOutput)
        {
            if (semiProbabilisticOutput == null)
            {
                throw new ArgumentNullException(nameof(semiProbabilisticOutput));
            }
            SemiProbabilisticOutput = semiProbabilisticOutput;
        }

        /// <summary>
        /// Gets the semi-probabilistic output created from the macro stability inwards output.
        /// </summary>
        public MacroStabilityInwardsSemiProbabilisticOutput SemiProbabilisticOutput { get; private set; }
    }
}