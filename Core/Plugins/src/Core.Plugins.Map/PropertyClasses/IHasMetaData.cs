// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using Core.Common.Gui.PropertyBag;
using Core.Plugins.Map.UITypeEditors;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// Interface for <see cref="IObjectProperties"/> with a meta data property.
    /// </summary>
    public interface IHasMetaData : IObjectProperties
    {
        /// <summary>
        /// Gets the selectable meta data attribute that is selected.
        /// </summary>
        SelectableMetaDataAttribute SelectedMetaDataAttribute { get; }

        /// <summary>
        /// Return the collection of available selectable meta data attributes.
        /// </summary>
        /// <returns>The collection of available selectable meta data attributes.</returns>
        IEnumerable<SelectableMetaDataAttribute> GetAvailableMetaDataAttributes();
    }
}