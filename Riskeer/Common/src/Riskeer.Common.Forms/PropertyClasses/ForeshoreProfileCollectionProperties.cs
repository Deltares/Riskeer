﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ForeshoreProfileCollection"/> for properties panel.
    /// </summary>
    public class ForeshoreProfileCollectionProperties : ObjectProperties<ForeshoreProfileCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForeshoreProfileCollection"/>.
        /// </summary>
        /// <param name="collection">The collection for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/>
        /// is <c>null</c>.</exception>
        public ForeshoreProfileCollectionProperties(ForeshoreProfileCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            data = collection;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ObservableCollectionWithSourcePath_SourcePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProfileCollectionProperties_SourcePath_Description))]
        public string SourcePath
        {
            get
            {
                return data.SourcePath;
            }
        }
    }
}