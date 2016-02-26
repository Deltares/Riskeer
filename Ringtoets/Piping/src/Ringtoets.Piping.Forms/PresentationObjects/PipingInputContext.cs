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
using System.Collections.Generic;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="PipingInput"/>
    /// and allowing for selecting a surfaceline or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class PipingInputContext : PipingContext<PipingInput>
    {
        public PipingInputContext(PipingInput pipingInput, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<PipingSoilProfile> soilProfiles, AssessmentSectionBase assessmentSection)
            : base(pipingInput, surfaceLines, soilProfiles, assessmentSection)
        { }
    }
}