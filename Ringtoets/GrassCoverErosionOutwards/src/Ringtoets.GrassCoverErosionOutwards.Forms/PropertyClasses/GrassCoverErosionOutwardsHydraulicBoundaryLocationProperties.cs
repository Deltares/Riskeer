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

using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionOutwardsHydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class GrassCoverErosionOutwardsHydraulicBoundaryLocationProperties : ObjectProperties<SectionSpecificWaterLevelHydraulicBoundaryLocationContext>
    {
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Id_Description")]
        public virtual long Id
        {
            get
            {
                return data.GrassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation.Id;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Name_Description")]
        public virtual string Name
        {
            get
            {
                return data.GrassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation.Name;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Location_Coordinates_Description")]
        public virtual Point2D Location
        {
            get
            {
                return data.GrassCoverErosionOutwardsHydraulicBoundaryLocation.HydraulicBoundaryLocation.Location;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryDatabase_Convergence_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryDatabase_Convergence_SectionSpecificWaterLevel_Description")]
        public virtual string Convergence
        {
            get
            {
                return new EnumDisplayWrapper<CalculationConvergence>(data.GrassCoverErosionOutwardsHydraulicBoundaryLocation.SectionSpecificWaterLevelCalculationConvergence).DisplayName;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Location);
        }
    }
}