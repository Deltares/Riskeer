// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.ChangeHandlers;
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
    /// ViewModel of <see cref="WaveConditionsInputContext{T}"/> for properties panel.
    /// </summary>
    /// <typeparam name="TContext">The type of the wave conditions input context.</typeparam>
    /// <typeparam name="TInput">The type of the contained wave conditions input.</typeparam>
    /// <typeparam name="TCategory">The category type contained by the wave conditions input.</typeparam>
    public abstract class WaveConditionsInputContextProperties<TContext, TInput, TCategory>
        : ObjectProperties<TContext>,
          IHasHydraulicBoundaryLocationProperty,
          IHasForeshoreProfileProperty
        where TContext : WaveConditionsInputContext<TInput>
        where TInput : WaveConditionsInput
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int categoryTypePropertyIndex = 1;
        private const int assessmentLevelPropertyIndex = 2;
        private const int upperBoundaryAssessmentLevelPropertyIndex = 3;
        private const int upperBoundaryRevetmentPropertyIndex = 4;
        private const int lowerBoundaryRevetmentPropertyIndex = 5;
        private const int upperBoundaryWaterLevelsPropertyIndex = 6;
        private const int lowerBoundaryWaterLevelsPropertyIndex = 7;
        private const int stepSizePropertyIndex = 8;
        private const int waterLevelsPropertyIndex = 9;

        private const int foreshoreProfilePropertyIndex = 10;
        private const int worldReferencePointPropertyIndex = 11;
        private const int orientationPropertyIndex = 12;
        private const int breakWaterPropertyIndex = 13;
        private const int foreshoreGeometryPropertyIndex = 14;
        private const int revetmentTypePropertyIndex = 15;

        private readonly Func<RoundedDouble> getAssessmentLevelFunc;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputContextProperties{TContext,TInput,TCategory}"/>.
        /// </summary>
        /// <param name="context">The <see cref="WaveConditionsInputContext{TInput}"/> for which the properties are shown.</param>
        /// <param name="getAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the assessment level.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected WaveConditionsInputContextProperties(TContext context,
                                                       Func<RoundedDouble> getAssessmentLevelFunc,
                                                       IObservablePropertyChangeHandler propertyChangeHandler)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (getAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getAssessmentLevelFunc));
            }

            if (propertyChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(propertyChangeHandler));
            }

            Data = context;

            this.getAssessmentLevelFunc = getAssessmentLevelFunc;
            this.propertyChangeHandler = propertyChangeHandler;
        }

        [PropertyOrder(categoryTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_CategoryType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_CategoryType_Description))]
        public TCategory CategoryType
        {
            get
            {
                return GetCategoryType();
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => SetCategoryType(value), propertyChangeHandler);
            }
        }

        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_Description))]
        public virtual RoundedDouble AssessmentLevel
        {
            get
            {
                return getAssessmentLevelFunc();
            }
        }

        [PropertyOrder(upperBoundaryAssessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryAssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryAssessmentLevel_Description))]
        public virtual RoundedDouble UpperBoundaryAssessmentLevel
        {
            get
            {
                return WaveConditionsInputHelper.GetUpperBoundaryAssessmentLevel(AssessmentLevel);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.UpperBoundaryRevetment = value, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.LowerBoundaryRevetment = value, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.UpperBoundaryWaterLevels = value, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.LowerBoundaryWaterLevels = value, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.StepSize = value, propertyChangeHandler);
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
                return data.WrappedData.GetWaterLevels(getAssessmentLevelFunc()).ToArray();
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
                return data.WrappedData.ForeshoreProfile == null
                           ? null
                           : new Point2D(new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.X),
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.Orientation = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.BreakWaterProperties_Description))]
        public UseBreakWaterProperties BreakWater
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null
                           ? new UseBreakWaterProperties()
                           : new UseBreakWaterProperties(data.WrappedData, propertyChangeHandler);
            }
        }

        [PropertyOrder(foreshoreGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ForeshoreProperties_Description))]
        public UseForeshoreProperties ForeshoreGeometry
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ForeshoreProfile = value, propertyChangeHandler);
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation, propertyChangeHandler);
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

        /// <summary>
        /// Gets the category type that is set to the wave conditions input.
        /// </summary>
        /// <returns>The category type at stake.</returns>
        protected abstract TCategory GetCategoryType();

        /// <summary>
        /// Sets the provided category type to the wave conditions input.
        /// </summary>
        /// <param name="categoryType">The category type to set.</param>
        protected abstract void SetCategoryType(TCategory categoryType);
    }
}