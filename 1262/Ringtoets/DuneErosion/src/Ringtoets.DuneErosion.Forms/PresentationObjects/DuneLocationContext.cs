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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.DuneErosion.Data;

namespace Ringtoets.DuneErosion.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of a <see cref="DuneLocation"/>
    /// </summary>
    public class DuneLocationContext : ObservableWrappedObjectContextBase<ObservableList<DuneLocation>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationContext"/>.
        /// </summary>
        /// <param name="wrappedList">The <see cref="ObservableList{T}"/> which the <see cref="DuneLocationContext"/>
        /// belongs to.</param>
        /// <param name="duneLocation">The <see cref="DuneLocation"/> which the <see cref="DuneLocationContext"/>
        /// belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationContext(ObservableList<DuneLocation> wrappedList, DuneLocation duneLocation)
            : base(wrappedList)
        {
            if (duneLocation == null)
            {
                throw new ArgumentNullException(nameof(duneLocation));
            }

            DuneLocation = duneLocation;
        }

        /// <summary>
        /// Gets the <see cref="Data.DuneLocation"/>.
        /// </summary>
        public DuneLocation DuneLocation { get; }

        public override bool Equals(WrappedObjectContextBase<ObservableList<DuneLocation>> other)
        {
            return base.Equals(other) && ReferenceEquals(((DuneLocationContext) other).DuneLocation, DuneLocation);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ DuneLocation.GetHashCode();
        }
    }
}