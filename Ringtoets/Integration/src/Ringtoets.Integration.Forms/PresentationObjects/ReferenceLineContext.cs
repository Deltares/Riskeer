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

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="ReferenceLine"/> instances.
    /// </summary>
    public class ReferenceLineContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceLineContext"/> class.
        /// </summary>
        /// <param name="referenceLine">The reference line.</param>
        /// <param name="parent">The parent owner of <paramref name="referenceLine"/>.</param>
        public ReferenceLineContext(ReferenceLine referenceLine, AssessmentSectionBase parent)
        {
            WrappedData = referenceLine;
            Parent = parent;
        }

        /// <summary>
        /// The reference line data wrapped by this presentation object.
        /// </summary>
        public ReferenceLine WrappedData { get; private set; }

        /// <summary>
        /// The assessment section owning <see cref="WrappedData"/>.
        /// </summary>
        public AssessmentSectionBase Parent { get; private set; }
    }
}