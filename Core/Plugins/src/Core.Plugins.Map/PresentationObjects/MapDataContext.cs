// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Controls.PresentationObjects;
using Core.Components.Gis.Data;

namespace Core.Plugins.Map.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="MapData"/>.
    /// </summary>
    public abstract class MapDataContext : ObservableWrappedObjectContextBase<MapData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapDataContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="MapData"/> to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wrappedData"/>
        /// is <c>null</c>.</exception>
        protected MapDataContext(MapData wrappedData)
            : base(wrappedData)
        {
        }

        /// <summary>
        /// Gets the parent <see cref="MapDataCollection"/>
        /// the <see cref="WrappedObjectContextBase{T}.WrappedData"/> belongs to.
        /// </summary>
        public abstract MapDataCollection ParentMapData { get; }
    }
}