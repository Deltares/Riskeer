// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.PresentationObjects;
using Riskeer.Revetment.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInputContext{T}"/> for properties panel.
    /// </summary>
    /// <typeparam name="TContext">The type of the wave conditions input context.</typeparam>
    /// <typeparam name="TInput">The type of the contained wave conditions input.</typeparam>
    /// <typeparam name="TCategory">The category type contained by the wave conditions input.</typeparam>
    /// <typeparam name="TCalculationType">The type of the calculation.</typeparam>
    public abstract class WaveConditionsInputContextProperties<TContext, TInput, TCategory, TCalculationType>
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

        private const int revetmentTypePropertyIndex = 10;

        private const int foreshoreProfilePropertyIndex = 11;
        private const int worldReferencePointPropertyIndex = 12;
        private const int orientationPropertyIndex = 13;
        private const int breakWaterPropertyIndex = 14;
        private const int foreshoreGeometryPropertyIndex = 15;

        private readonly Func<RoundedDouble> getAssessmentLevelFunc;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsInputContextProperties{TContext,TInput,TCategory, TCalculationType}"/>.
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
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => SetCategoryType(value));
            }
        }

        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_AssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_AssessmentLevel_Description))]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return getAssessmentLevelFunc();
            }
        }

        [PropertyOrder(upperBoundaryAssessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryAssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_UpperBoundaryAssessmentLevel_Description))]
        public RoundedDouble UpperBoundaryAssessmentLevel
        {
            get
            {
                return WaveConditionsInputHelper.GetUpperBoundaryAssessmentLevel(AssessmentLevel);
            }
        }

        [PropertyOrder(upperBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => data.WrappedData.UpperBoundaryRevetment = value);
            }
        }

        [PropertyOrder(lowerBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => data.WrappedData.LowerBoundaryRevetment = value);
            }
        }

        [PropertyOrder(upperBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => data.WrappedData.UpperBoundaryWaterLevels = value);
            }
        }

        [PropertyOrder(lowerBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => data.WrappedData.LowerBoundaryWaterLevels = value);
            }
        }

        [PropertyOrder(stepSizePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
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
                HandleChangeProperty(() => data.WrappedData.StepSize = value);
            }
        }

        [PropertyOrder(waterLevelsPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_WaterLevels_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_WaterLevels_Description))]
        public RoundedDouble[] WaterLevels
        {
            get
            {
                return data.WrappedData.GetWaterLevels(getAssessmentLevelFunc()).ToArray();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(revetmentTypePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaveConditionsInput_RevetmentType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaveConditionsInput_RevetmentType_Description))]
        public abstract TCalculationType RevetmentType { get; set; }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WorldReferencePoint_ForeshoreProfile_Description))]
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
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Orientation_ForeshoreProfile_Description))]
        public RoundedDouble Orientation
        {
            get
            {
                return data.WrappedData.Orientation;
            }
            set
            {
                HandleChangeProperty(() => data.WrappedData.Orientation = value);
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_Description))]
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
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_Description))]
        public UseForeshoreProperties ForeshoreGeometry
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData, propertyChangeHandler);
            }
        }

        [PropertyOrder(foreshoreProfilePropertyIndex)]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
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
                HandleChangeProperty(() => data.WrappedData.ForeshoreProfile = value);
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_Description))]
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
                HandleChangeProperty(() => data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public virtual bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(RevetmentType))
            {
                return true;
            }

            return false;
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

        /// <summary>
        /// Handles the property change.
        /// </summary>
        /// <param name="setPropertyDelegate">The property change action.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="setPropertyDelegate"/>
        /// is <c>null</c>.</exception>
        protected void HandleChangeProperty(SetObservablePropertyValueDelegate setPropertyDelegate)
        {
            PropertyChangeHelper.ChangePropertyAndNotify(setPropertyDelegate, propertyChangeHandler);
        }
    }
}