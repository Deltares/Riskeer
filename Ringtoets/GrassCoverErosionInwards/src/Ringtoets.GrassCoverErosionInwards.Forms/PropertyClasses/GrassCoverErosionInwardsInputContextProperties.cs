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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Utils;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>,
                                                                  IHasHydraulicBoundaryLocationProperty
    {
        private const int dikeProfilePropertyIndex = 1;
        private const int worldReferencePointPropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int foreshorePropertyIndex = 5;
        private const int dikeGeometryPropertyIndex = 6;
        private const int dikeHeightPropertyIndex = 7;
        private const int criticalFlowRatePropertyIndex = 8;
        private const int hydraulicBoundaryLocationPropertyIndex = 9;
        private const int calculateDikeHeightPropertyIndex = 10;

        private readonly ICalculationInputPropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsInputContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public GrassCoverErosionInwardsInputContextProperties(GrassCoverErosionInwardsInputContext data,
                                                              ICalculationInputPropertyChangeHandler handler)
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeProfile_Description))]
        public DikeProfile DikeProfile
        {
            get
            {
                return data.WrappedData.DikeProfile;
            }
            set
            {
                ChangePropertyAndNotify((input, v) =>
                {
                    input.DikeProfile = v;
                    GrassCoverErosionInwardsHelper.UpdateCalculationToSectionResultAssignments(
                        data.FailureMechanism.SectionResults,
                        data.FailureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>());
                }, value);
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WorldReferencePoint_DikeProfile_Description))]
        public Point2D WorldReferencePoint
        {
            get
            {
                return data.WrappedData.DikeProfile == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.Y));
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Orientation_DikeProfile_Description))]
        public RoundedDouble Orientation
        {
            get
            {
                return data.WrappedData.Orientation;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => input.Orientation = newValue, value);
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_Description))]
        public UseBreakWaterProperties<GrassCoverErosionInwardsInput> BreakWater
        {
            get
            {
                return data.WrappedData.DikeProfile == null ?
                           new UseBreakWaterProperties<GrassCoverErosionInwardsInput>() :
                           new UseBreakWaterProperties<GrassCoverErosionInwardsInput>(
                               data.WrappedData, data.Calculation, propertyChangeHandler);
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_Description))]
        public UseForeshoreProperties<GrassCoverErosionInwardsInput> Foreshore
        {
            get
            {
                return new UseForeshoreProperties<GrassCoverErosionInwardsInput>(
                    data.WrappedData,
                    data.Calculation,
                    propertyChangeHandler);
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.DikeHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeight_Description))]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.WrappedData.DikeHeight;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => input.DikeHeight = newValue, value);
            }
        }

        [PropertyOrder(calculateDikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeHeightCalculationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeightCalculationType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public DikeHeightCalculationType DikeHeightCalculationType
        {
            get
            {
                return data.WrappedData.DikeHeightCalculationType;
            }
            set
            {
                ChangePropertyAndNotify(
                    (input, newValue) => input.DikeHeightCalculationType = newValue, value);
            }
        }

        [PropertyOrder(criticalFlowRatePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_CriticalValues))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.CriticalFlowRate_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.CriticalFlowRate_Description))]
        public ConfirmingLogNormalDistributionProperties<GrassCoverErosionInwardsInput> CriticalFlowRate
        {
            get
            {
                return new ConfirmingLogNormalDistributionProperties<GrassCoverErosionInwardsInput>(
                    DistributionPropertiesReadOnly.None,
                    data.WrappedData.CriticalFlowRate,
                    data.Calculation,
                    data.WrappedData,
                    propertyChangeHandler);
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryLocation_Description))]
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
                ChangePropertyAndNotify(
                    (input, newValue) => input.HydraulicBoundaryLocation = newValue,
                    value.HydraulicBoundaryLocation);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data.WrappedData.DikeProfile == null;
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
                data.AvailableHydraulicBoundaryLocations, calculationLocation);
        }

        private void ChangePropertyAndNotify<TValue>(
            SetCalculationInputPropertyValueDelegate<GrassCoverErosionInwardsInput, TValue> setPropertyValue,
            TValue value)
        {
            IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                data.WrappedData,
                data.Calculation,
                value,
                setPropertyValue);

            NotifyAffectedObjects(affectedObjects);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}