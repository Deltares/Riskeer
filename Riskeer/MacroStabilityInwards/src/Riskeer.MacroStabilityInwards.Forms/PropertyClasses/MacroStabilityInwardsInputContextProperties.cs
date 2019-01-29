// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Forms.UITypeEditors;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsInputContext"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsInputContextProperties : ObjectProperties<MacroStabilityInwardsInputContext>,
                                                               IHasHydraulicBoundaryLocationProperty
    {
        private const int selectedHydraulicBoundaryLocationPropertyIndex = 1;
        private const int assessmentLevelPropertyIndex = 2;
        private const int useHydraulicBoundaryLocationPropertyIndex = 3;
        private const int dikeSoilScenarioPropertyIndex = 4;
        private const int waterStressesPropertyIndex = 5;
        private const int surfaceLinePropertyIndex = 6;
        private const int stochasticSoilModelPropertyIndex = 7;
        private const int stochasticSoilProfilePropertyIndex = 8;
        private const int slipPlaneMinimumDepthPropertyIndex = 9;
        private const int slipPlaneMinimumLengthPropertyIndex = 10;
        private const int maximumSliceWidthPropertyIndex = 11;
        private const int slipPlaneSettingsPropertyIndex = 12;
        private const int gridSettingsPropertyIndex = 13;

        private const int hydraulicCategoryIndex = 1;
        private const int schematizationCategoryIndex = 2;
        private const int settingsCategoryIndex = 3;
        private const int totalCategoryCount = 3;

        private readonly Func<RoundedDouble> getNormativeAssessmentLevelFunc;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsInputContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties for.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the normative assessment level.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsInputContextProperties(MacroStabilityInwardsInputContext data,
                                                           Func<RoundedDouble> getNormativeAssessmentLevelFunc,
                                                           IObservablePropertyChangeHandler propertyChangeHandler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            if (propertyChangeHandler == null)
            {
                throw new ArgumentNullException(nameof(this.propertyChangeHandler));
            }

            Data = data;

            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;
            this.propertyChangeHandler = propertyChangeHandler;
        }

        /// <summary>
        /// Gets the available surface lines on <see cref="MacroStabilityInwardsCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailableMacroStabilityInwardsSurfaceLines;
        }

        /// <summary>
        /// Gets the available stochastic soil models on <see cref="MacroStabilityInwardsCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsStochasticSoilModel> GetAvailableStochasticSoilModels()
        {
            if (data.WrappedData.SurfaceLine == null)
            {
                return data.AvailableStochasticSoilModels;
            }

            return MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(data.WrappedData.SurfaceLine, data.AvailableStochasticSoilModels);
        }

        /// <summary>
        /// Gets the available stochastic soil profiles on <see cref="MacroStabilityInwardsCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsStochasticSoilProfile> GetAvailableStochasticSoilProfiles()
        {
            return data.WrappedData.StochasticSoilModel != null ? data.WrappedData.StochasticSoilModel.StochasticSoilProfiles : new List<MacroStabilityInwardsStochasticSoilProfile>();
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(AssessmentLevel))
            {
                return !data.WrappedData.UseAssessmentLevelManualInput;
            }

            return true;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(SelectedHydraulicBoundaryLocation))
            {
                return !data.WrappedData.UseAssessmentLevelManualInput;
            }

            return false;
        }

        /// <summary>
        /// Gets the available selectable hydraulic boundary locations on <see cref="MacroStabilityInwardsInputContext"/>.
        /// </summary>
        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referencePoint = SurfaceLine?.ReferenceLineIntersectionWorldPoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AssessmentSection.HydraulicBoundaryDatabase.Locations, referencePoint);
        }

        private RoundedDouble GetEffectiveAssessmentLevel()
        {
            return data.WrappedData.UseAssessmentLevelManualInput
                       ? data.WrappedData.AssessmentLevel
                       : getNormativeAssessmentLevelFunc();
        }

        #region Hydraulic data

        [DynamicVisible]
        [PropertyOrder(selectedHydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), hydraulicCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryLocation_Description_with_assessment_level))]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                Point2D referencePoint = SurfaceLine?.ReferenceLineIntersectionWorldPoint;

                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation,
                                                                     referencePoint)
                           : null;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), hydraulicCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.AssessmentLevel_Description))]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return GetEffectiveAssessmentLevel();
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.AssessmentLevel = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(useHydraulicBoundaryLocationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), hydraulicCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.UseAssessmentLevelManualInput_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.UseAssessmentLevelManualInput_Description))]
        public bool UseAssessmentLevelManualInput
        {
            get
            {
                return data.WrappedData.UseAssessmentLevelManualInput;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.UseAssessmentLevelManualInput = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(dikeSoilScenarioPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), hydraulicCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeSoilScenario_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeSoilScenario_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsDikeSoilScenario DikeSoilScenario
        {
            get
            {
                return data.WrappedData.DikeSoilScenario;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.DikeSoilScenario = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(waterStressesPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), hydraulicCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Waterstresses_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsWaterStressesProperties WaterStressesProperties
        {
            get
            {
                return new MacroStabilityInwardsWaterStressesProperties(data.WrappedData, GetEffectiveAssessmentLevel(), propertyChangeHandler);
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(surfaceLinePropertyIndex)]
        [Editor(typeof(MacroStabilityInwardsInputContextSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), schematizationCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Description))]
        public MacroStabilityInwardsSurfaceLine SurfaceLine
        {
            get
            {
                return data.WrappedData.SurfaceLine;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.SurfaceLine))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() =>
                    {
                        data.WrappedData.SurfaceLine = value;
                        MacroStabilityInwardsInputService.SetMatchingStochasticSoilModel(data.WrappedData, GetAvailableStochasticSoilModels());
                    }, propertyChangeHandler);
                }
            }
        }

        [PropertyOrder(stochasticSoilModelPropertyIndex)]
        [Editor(typeof(MacroStabilityInwardsInputContextStochasticSoilModelSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), schematizationCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsInput_StochasticSoilModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsInput_StochasticSoilModel_Description))]
        public MacroStabilityInwardsStochasticSoilModel StochasticSoilModel
        {
            get
            {
                return data.WrappedData.StochasticSoilModel;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.StochasticSoilModel))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() =>
                    {
                        data.WrappedData.StochasticSoilModel = value;
                        MacroStabilityInwardsInputService.SyncStochasticSoilProfileWithStochasticSoilModel(data.WrappedData);
                    }, propertyChangeHandler);
                }
            }
        }

        [PropertyOrder(stochasticSoilProfilePropertyIndex)]
        [Editor(typeof(MacroStabilityInwardsInputContextStochasticSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), schematizationCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MacroStabilityInwardsInput_StochasticSoilProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MacroStabilityInwardsInput_StochasticSoilProfile_Description))]
        public MacroStabilityInwardsStochasticSoilProfile StochasticSoilProfile
        {
            get
            {
                return data.WrappedData.StochasticSoilProfile;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.StochasticSoilProfile))
                {
                    PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.StochasticSoilProfile = value, propertyChangeHandler);
                }
            }
        }

        #endregion

        #region Settings

        [PropertyOrder(slipPlaneMinimumDepthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Settings), settingsCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SlipPlaneMinimumDepth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SlipPlaneMinimumDepth_Description))]
        public RoundedDouble SlipPlaneMinimumDepth
        {
            get
            {
                return data.WrappedData.SlipPlaneMinimumDepth;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.SlipPlaneMinimumDepth = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(slipPlaneMinimumLengthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Settings), settingsCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SlipPlaneMinimumLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SlipPlaneMinimumLength_Description))]
        public RoundedDouble SlipPlaneMinimumLength
        {
            get
            {
                return data.WrappedData.SlipPlaneMinimumLength;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.SlipPlaneMinimumLength = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(maximumSliceWidthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Settings), settingsCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MaximumSliceWidth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MaximumSliceWidth_Description))]
        public RoundedDouble MaximumSliceWidth
        {
            get
            {
                return data.WrappedData.MaximumSliceWidth;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.MaximumSliceWidth = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(slipPlaneSettingsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Settings), settingsCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SlipPlaneSettings_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.SlipPlaneSettings_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsSlipPlaneSettingsProperties SlipPlaneSettings
        {
            get
            {
                return new MacroStabilityInwardsSlipPlaneSettingsProperties(data.WrappedData, propertyChangeHandler);
            }
        }

        [PropertyOrder(gridSettingsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Settings), settingsCategoryIndex, totalCategoryCount)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GridSettings_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsGridSettingsProperties GridSettings
        {
            get
            {
                return new MacroStabilityInwardsGridSettingsProperties(data.WrappedData, propertyChangeHandler);
            }
        }

        #endregion
    }
}