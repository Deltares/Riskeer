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

using System.Collections.Generic;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Model for performing GrassCoverErosionInwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanism : FailureMechanismBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism()
            : base(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName) {}

        public override IEnumerable<ICalculationItem> CalculationItems
        {
            get
            {
                yield break;
            }
        }
    }
}