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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInputContext"/> for properties panel.
    /// </summary>
    public abstract class WaveConditionsInputContextProperties<T> : ObjectProperties<T>,
                                                                    IHasHydraulicBoundaryLocationProperty,
                                                                    IHasForeshoreProfileProperty
        where T : WaveConditionsInputContext
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int assessmentLevelPropertyIndex = 1;
        private const int upperBoundaryDesignWaterLevelPropertyIndex = 2;
        private const int upperBoundaryRevetmentPropertyIndex = 3;
        private const int lowerBoundaryRevetmentPropertyIndex = 4;
        private const int upperBoundaryWaterLevelsPropertyIndex = 5;
        private const int lowerBoundaryWaterLevelsPropertyIndex = 6;
        private const int stepSizePropertyIndex = 7;
        private const int waterLevelsPropertyIndex = 8;

        private const int foreshoreProfilePropertyIndex = 9;
        private const int worldReferencePointPropertyIndex = 10;
        private const int orientationPropertyIndex = 11;
        private const int breakWaterPropertyIndex = 12;
        private const int foreshoreGeometryPropertyIndex = 13;
        private const int revetmentTypePropertyIndex = 14;

        private readonly ICalculationInputPropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputContextProperties{T}"/>.
        /// </summary>
        /// <param name="context">The <see cref="WaveConditionsInputContext"/> for which the properties are shown.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected WaveConditionsInputContextProperties(T context, ICalculationInputPropertyChangeHandler handler)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            propertyChangeHandler = handler;
            Data = context;
        }

        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_Description))]
        public virtual RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
        }

        [PropertyOrder(upperBoundaryDesignWaterLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryDesignWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryDesignWaterLevel_Description))]
        public virtual RoundedDouble UpperBoundaryDesignWaterLevel
        {
            get
            {
                return data.WrappedData.UpperBoundaryDesignWaterLevel;
            }
        }

        [PropertyOrder(upperBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryRevetment_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryRevetment_Description))]
        public RoundedDouble UpperBoundaryRevetment
        {
            get
            {
                return data.WrappedData.UpperBoundaryRevetment;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.UpperBoundaryRevetment = newValue, value);
            }
        }

        [PropertyOrder(lowerBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_LowerBoundaryRevetment_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_LowerBoundaryRevetment_Description))]
        public RoundedDouble LowerBoundaryRevetment
        {
            get
            {
                return data.WrappedData.LowerBoundaryRevetment;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.LowerBoundaryRevetment = newValue, value);
            }
        }

        [PropertyOrder(upperBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryWaterLevels_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryWaterLevels_Description))]
        public RoundedDouble UpperBoundaryWaterLevels
        {
            get
            {
                return data.WrappedData.UpperBoundaryWaterLevels;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.UpperBoundaryWaterLevels = newValue, value);
            }
        }

        [PropertyOrder(lowerBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_LowerBoundaryWaterLevels_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_LowerBoundaryWaterLevels_Description))]
        public RoundedDouble LowerBoundaryWaterLevels
        {
            get
            {
                return data.WrappedData.LowerBoundaryWaterLevels;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.LowerBoundaryWaterLevels = newValue, value);
            }
        }

        [PropertyOrder(stepSizePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_StepSize_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_StepSize_Description))]
        public WaveConditionsInputStepSize StepSize
        {
            get
            {
                return data.WrappedData.StepSize;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.StepSize = newValue, value);
            }
        }

        [PropertyOrder(waterLevelsPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_WaterLevels_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_WaterLevels_Description))]
        public RoundedDouble[] WaterLevels
        {
            get
            {
                return data.WrappedData.WaterLevels.ToArray();
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.WorldReferencePoint_ForeshoreProfile_Description))]
        public Point2D WorldReferencePoint
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.Y));
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Orientation_ForeshoreProfile_Description))]
        public RoundedDouble Orientation
        {
            get
            {
                return data.WrappedData.Orientation;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.Orientation = newValue, value);
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_Description))]
        public UseBreakWaterProperties<WaveConditionsInput> BreakWater
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null ?
                           new UseBreakWaterProperties<WaveConditionsInput>() :
                           new UseBreakWaterProperties<WaveConditionsInput>(data.WrappedData, data.Calculation, propertyChangeHandler);
            }
        }

        [PropertyOrder(foreshoreGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_Description))]
        public UseForeshoreProperties<WaveConditionsInput> ForeshoreGeometry
        {
            get
            {
                return new UseForeshoreProperties<WaveConditionsInput>(data.WrappedData, data.Calculation, propertyChangeHandler);
            }
        }

        [PropertyOrder(revetmentTypePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_RevetmentType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_RevetmentType_Description))]
        public abstract string RevetmentType { get; }

        [PropertyOrder(foreshoreProfilePropertyIndex)]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ForeshoreProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ForeshoreProfile_Description))]
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return data.WrappedData.ForeshoreProfile;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.ForeshoreProfile = newValue, value);
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.HydraulicBoundaryLocation_Description))]
        public virtual SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                Point2D referenceLocation = data.WrappedData.ForeshoreProfile?.WorldReferencePoint;
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation,
                                                                     referenceLocation)
                           : null;
            }
            set
            {
                ChangePropertyAndNotify((input, newValue) => input.HydraulicBoundaryLocation = newValue.HydraulicBoundaryLocation, value);
            }
        }

        public virtual IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.ForeshoreProfiles;
        }

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referenceLocation = data.WrappedData.ForeshoreProfile?.WorldReferencePoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.HydraulicBoundaryLocations, referenceLocation);
        }

        private void ChangePropertyAndNotify<TValue>(
            SetCalculationInputPropertyValueDelegate<WaveConditionsInput, TValue> setPropertyValue,
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
            foreach (var affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}