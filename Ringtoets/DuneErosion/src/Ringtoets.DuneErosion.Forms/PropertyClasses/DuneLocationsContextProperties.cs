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

using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Properties;
using RingtoetsCommonFormsProperties = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="DuneLocationsContext"/> for the properties panel.
    /// </summary>
    public class DuneLocationsContextProperties : ObjectProperties<DuneLocationsContext>
    {
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsProperties), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "DuneLocationsContextProperties_Locations_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DuneLocationsContextProperties_Locations_Description")]
        public DuneLocationProperties[] Locations
        {
            get
            {
                return data.WrappedData.Select(duneLocation => new DuneLocationProperties
                {
                    Data = duneLocation
                }).ToArray();
            }
        }
    }
}