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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        private const int orientationPropertyIndex = 1;
        private const int breakWaterPropertyIndex = 2;
        private const int foreshorePropertyIndex = 3;
        private const int dikeGeometryPropertyIndex = 4;
        private const int dikeHeightPropertyIndex = 5;
        private const int criticalFlowRatePropertyIndex = 6;
        private const int hydraulicBoundaryLocationPropertyIndex = 7;

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Orientation_Description")]
        public RoundedDouble Orientation
        {
            get
            {
                return data.WrappedData.Orientation;
            }
            set
            {
                data.WrappedData.Orientation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterProperties_Description")]
        public BreakWaterProperties BreakWater
        {
            get
            {
                return new BreakWaterProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProperties_Description")]
        public ForeshoreProperties Foreshore
        {
            get
            {
                return new ForeshoreProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometryProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometryProperties_Description")]
        public DikeGeometryProperties DikeGeometry
        {
            get
            {
                return new DikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeHeight_Description")]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.WrappedData.DikeHeight;
            }
            set
            {
                data.WrappedData.DikeHeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(criticalFlowRatePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_CriticalValues")]
        [ResourcesDisplayName(typeof(Resources), "CriticalFlowRate_DisplayName")]
        [ResourcesDescription(typeof(Resources), "CriticalFlowRate_Description")]
        public LogNormalDistributionProperties CriticalFlowRate
        {
            get
            {
                return new LogNormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.CriticalFlowRate
                };
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(GrassCoverErosionInwardsInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryLocation_Description")]
        public HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }
    }
}