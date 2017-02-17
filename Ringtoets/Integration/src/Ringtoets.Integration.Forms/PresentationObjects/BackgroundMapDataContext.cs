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
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="IAssessmentSection.BackgroundMapData"/>.
    /// </summary>
    public class BackgroundMapDataContext : ObservableWrappedObjectContextBase<WmtsMapData>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BackgroundMapDataContext"/>.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <param name="backgroundMapData">The <see cref="BackgroundMapDataContainer"/> 
        /// which the <see cref="BackgroundMapDataContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>
        /// is <c>null</c>.</exception>
        public BackgroundMapDataContext(WmtsMapData wrappedData, BackgroundMapDataContainer backgroundMapData) : base(wrappedData)
        {
            if (backgroundMapData == null)
            {
                throw new ArgumentNullException(nameof(backgroundMapData));
            }
            BackgroundMapData = backgroundMapData;
        }

        /// <summary>
        /// Gets the <see cref="BackgroundMapDataContainer"/>. 
        /// </summary>
        public BackgroundMapDataContainer BackgroundMapData { get; private set; }

        #region IEquatable members

        public override bool Equals(WrappedObjectContextBase<WmtsMapData> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (other.GetType() != GetType())
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return ReferenceEquals(((BackgroundMapDataContext) other).BackgroundMapData, BackgroundMapData);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ BackgroundMapData.GetHashCode();
        }

        #endregion
    }
}