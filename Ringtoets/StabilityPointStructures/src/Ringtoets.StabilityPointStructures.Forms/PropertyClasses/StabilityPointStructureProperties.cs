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

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructure"/> for properties panel.
    /// </summary>
    public class StabilityPointStructureProperties : ObjectProperties<StabilityPointStructure>
    {
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(locationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Location_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Location_Description")]
        public Point2D Location
        {
            get
            {
                return new Point2D(new RoundedDouble(0, data.Location.X),
                                   new RoundedDouble(0, data.Location.Y));
            }
        }
    }
}