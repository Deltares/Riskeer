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
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Override of <see cref="GeneralResultFaultTreeIllustrationPointView"/> for making output views for
    /// <see cref="OvertoppingRateOutput"/> uniquely identifiable (when it comes to opening/closing views).
    /// </summary>
    public class OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView : GeneralResultFaultTreeIllustrationPointView
    {
        /// <summary>
        /// Creates a new instance of <see cref="OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView"/>.
        /// </summary>
        /// <param name="getGeneralResultFunc">A <see cref="Func{TResult}"/> for obtaining the illustration point
        /// data (<see cref="GeneralResult{T}"/> with <see cref="TopLevelFaultTreeIllustrationPoint"/> objects)
        /// that must be presented.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="getGeneralResultFunc"/> is <c>null</c>.</exception>
        public OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView(Func<GeneralResult<TopLevelFaultTreeIllustrationPoint>> getGeneralResultFunc)
            : base(getGeneralResultFunc) {}
    }
}