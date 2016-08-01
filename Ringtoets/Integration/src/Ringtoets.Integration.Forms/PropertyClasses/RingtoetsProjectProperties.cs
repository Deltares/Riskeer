﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// This class describes the presentation of properties of a <see cref="IProject"/>.
    /// </summary>
    public class RingtoetsProjectProperties : ObjectProperties<IProject>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsIntegrationFormsResources), "RingtoetsProjectProperties_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsIntegrationFormsResources), "RingtoetsProjectProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsIntegrationFormsResources), "RingtoetsProjectProperties_Description_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsIntegrationFormsResources), "RingtoetsProjectProperties_Description_Description")]
        public string Description
        {
            get
            {
                return data.Description;
            }
            set
            {
                data.Description = value;
            }
        }
    }
}