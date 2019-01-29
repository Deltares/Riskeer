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

using Core.Common.Controls.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for the dike height output of grass cover erosion inwards calculations.
    /// </summary>
    public class DikeHeightOutputContext : ObservableWrappedObjectContextBase<GrassCoverErosionInwardsCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DikeHeightOutputContext"/>.
        /// </summary>
        /// <param name="wrappedData">The grass cover erosion inwards calculation wrapped by the context object.</param>
        public DikeHeightOutputContext(GrassCoverErosionInwardsCalculation wrappedData) : base(wrappedData) {}
    }
}