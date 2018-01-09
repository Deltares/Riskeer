﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSection"/> for properties panel.
    /// </summary>
    public class FailureMechanismSectionsProperties : ObjectProperties<IEnumerable<FailureMechanismSection>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsProperties"/>.
        /// </summary>
        /// <param name="sections">The sections to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sections"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionsProperties(IEnumerable<FailureMechanismSection> sections)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            Data = sections;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSections_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FailureMechanismSections_Description))]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        public FailureMechanismSectionProperties[] Sections
        {
            get
            {
                return data.Select(section => new FailureMechanismSectionProperties(section)).ToArray();
            }
        }
    }
}