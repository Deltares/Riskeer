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
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object which wraps a <see cref="PipingOutput"/> and a <see cref="PipingSemiProbabilisticOutput"/>.
    /// </summary>
    public class PipingOutputContext: WrappedObjectContextBase<PipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingOutputContext"/>.
        /// </summary>
        /// <param name="wrappedData"></param>
        /// <param name="semiProbabilisticOutput"></param>
        public PipingOutputContext(PipingOutput wrappedData, PipingSemiProbabilisticOutput semiProbabilisticOutput) : base(wrappedData)
        {
            if (semiProbabilisticOutput == null)
            {
                throw new ArgumentNullException("semiProbabilisticOutput");
            }
            SemiProbabilisticOutput = semiProbabilisticOutput;
        }

        public PipingSemiProbabilisticOutput SemiProbabilisticOutput { get; private set; }
    }
}