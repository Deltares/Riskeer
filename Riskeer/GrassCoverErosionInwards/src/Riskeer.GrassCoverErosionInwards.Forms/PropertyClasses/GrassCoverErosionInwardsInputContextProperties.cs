﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using Riskeer.GrassCoverErosionInwards.Forms.UITypeEditors;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>,
                                                                  IHasHydraulicBoundaryLocationProperty
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 1;
        private const int dikeProfilePropertyIndex = 2;
        private const int worldReferencePointPropertyIndex = 3;
        private const int orientationPropertyIndex = 4;
        private const int breakWaterPropertyIndex = 5;
        private const int foreshorePropertyIndex = 6;
        private const int dikeGeometryPropertyIndex = 7;
        private const int dikeHeightPropertyIndex = 8;
        private const int criticalFlowRatePropertyIndex = 9;
        private const int shouldOvertoppingOutputIllustrationPointsBeCalculatedPropertyIndex = 10;
        private const int shouldDikeHeightBeCalculatedPropertyIndex = 11;
        private const int shouldDikeHeightIllustrationPointsBeCalculatedPropertyIndex = 12;
        private const int calculateOvertoppingRatePropertyIndex = 13;
        private const int shouldOvertoppingRateIllustrationPointsBeCalculatedPropertyIndex = 14;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsInputContextProperties(GrassCoverErosionInwardsInputContext data,
                                                              IObservablePropertyChangeHandler handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Data = data;
            propertyChangeHandler = handler;
        }

        [PropertyOrder(dikeProfilePropertyIndex)]
        [Editor(typeof(GrassCoverErosionInwardsInputContextDikeProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeProfile_Description))]
        public DikeProfile DikeProfile
        {
            get => data.WrappedData.DikeProfile;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.DikeProfile = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WorldReferencePoint_DikeProfile_Description))]
        public Point2D WorldReferencePoint
        {
            get
            {
                return data.WrappedData.DikeProfile == null
                           ? null
                           : new Point2D(
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.Y));
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Orientation_DikeProfile_Description))]
        public RoundedDouble Orientation
        {
            get => data.WrappedData.Orientation;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.Orientation = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_Description))]
        public UseBreakWaterProperties BreakWater
        {
            get
            {
                return data.WrappedData.DikeProfile == null
                           ? new UseBreakWaterProperties()
                           : new UseBreakWaterProperties(
                               data.WrappedData, propertyChangeHandler);
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_Description))]
        public UseForeshoreProperties Foreshore
        {
            get
            {
                return new UseForeshoreProperties(
                    data.WrappedData,
                    propertyChangeHandler);
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeGeometryProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeGeometryProperties_Description))]
        public GrassCoverErosionInwardsInputContextDikeGeometryProperties DikeGeometry
        {
            get
            {
                return new GrassCoverErosionInwardsInputContextDikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.DikeHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeight_Description))]
        public RoundedDouble DikeHeight
        {
            get => data.WrappedData.DikeHeight;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.DikeHeight = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(criticalFlowRatePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_CriticalValues), 3, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CriticalFlowRate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CriticalFlowRate_Description))]
        public LogNormalDistributionProperties CriticalFlowRate
        {
            get
            {
                return new LogNormalDistributionProperties(
                    DistributionReadOnlyProperties.None,
                    data.WrappedData.CriticalFlowRate,
                    propertyChangeHandler);
            }
        }

        [PropertyOrder(shouldOvertoppingOutputIllustrationPointsBeCalculatedPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingOutput), 4, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldOvertoppingOutputIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldOvertoppingOutputIllustrationPointsBeCalculated
        {
            get => data.WrappedData.ShouldOvertoppingOutputIllustrationPointsBeCalculated;
            set
            {
                data.WrappedData.ShouldOvertoppingOutputIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(shouldDikeHeightBeCalculatedPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_DikeHeight), 5, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldDikeHeightBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ShouldDikeHeightBeCalculated_Description))]
        public bool ShouldDikeHeightBeCalculated
        {
            get => data.WrappedData.ShouldDikeHeightBeCalculated;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ShouldDikeHeightBeCalculated = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(shouldDikeHeightIllustrationPointsBeCalculatedPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_DikeHeight), 5, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldDikeHeightIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldDikeHeightIllustrationPointsBeCalculated
        {
            get => data.WrappedData.ShouldDikeHeightIllustrationPointsBeCalculated;
            set
            {
                data.WrappedData.ShouldDikeHeightIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(calculateOvertoppingRatePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingRate), 6, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.OvertoppingRateCalculationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.OvertoppingRateCalculationType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public OvertoppingRateCalculationType OvertoppingRateCalculationType
        {
            get => data.WrappedData.OvertoppingRateCalculationType;
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.OvertoppingRateCalculationType = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(shouldOvertoppingRateIllustrationPointsBeCalculatedPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_OvertoppingRate), 6, 6)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ShouldOvertoppingRateIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldOvertoppingRateIllustrationPointsBeCalculated
        {
            get => data.WrappedData.ShouldOvertoppingRateIllustrationPointsBeCalculated;
            set
            {
                data.WrappedData.ShouldOvertoppingRateIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), 1, 6)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_Description))]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                Point2D referencePoint = data.WrappedData.DikeProfile?.WorldReferencePoint;
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation, referencePoint)
                           : null;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation, propertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName.Equals(nameof(Orientation)) || propertyName.Equals(nameof(DikeHeight)))
            {
                return data.WrappedData.DikeProfile == null;
            }

            if (propertyName.Equals(nameof(ShouldDikeHeightIllustrationPointsBeCalculated)))
            {
                return !ShouldDikeHeightBeCalculated;
            }

            if (propertyName.Equals(nameof(ShouldOvertoppingRateIllustrationPointsBeCalculated)))
            {
                return OvertoppingRateCalculationType == OvertoppingRateCalculationType.NoCalculation;
            }

            return false;
        }

        /// <summary>
        /// Returns the collection of available dike profiles.
        /// </summary>
        /// <returns>A collection of dike profiles.</returns>
        public IEnumerable<DikeProfile> GetAvailableDikeProfiles()
        {
            return data.AvailableDikeProfiles;
        }

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D calculationLocation = data.WrappedData.DikeProfile?.WorldReferencePoint;

            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AssessmentSection.HydraulicBoundaryDatabase.Locations, calculationLocation);
        }
    }
}