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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.UITypeEditors;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingInputContext"/> for properties panel.
    /// </summary>
    public class PipingInputContextProperties : ObjectProperties<PipingInputContext>,
                                                IHasHydraulicBoundaryLocationProperty
    {
        private const int selectedHydraulicBoundaryLocationPropertyIndex = 1;
        private const int assessmentLevelPropertyIndex = 2;
        private const int useHydraulicBoundaryLocationPropertyIndex = 3;
        private const int dampingFactorExitPropertyIndex = 4;
        private const int phreaticLevelExitPropertyIndex = 5;
        private const int piezometricHeadExitPropertyIndex = 6;
        private const int surfaceLinePropertyIndex = 7;
        private const int stochasticSoilModelPropertyIndex = 8;
        private const int stochasticSoilProfilePropertyIndex = 9;
        private const int entryPointLPropertyIndex = 10;
        private const int exitPointLPropertyIndex = 11;
        private const int seepageLengthPropertyIndex = 12;
        private const int thicknessCoverageLayerPropertyIndex = 13;
        private const int effectiveThicknessCoverageLayerPropertyIndex = 14;
        private const int thicknessAquiferLayerPropertyIndex = 15;
        private const int darcyPermeabilityPropertyIndex = 16;
        private const int diameter70PropertyIndex = 17;
        private const int saturatedVolumicWeightOfCoverageLayerPropertyIndex = 18;

        /// <summary>
        /// Gets the available surface lines on <see cref="PipingCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailablePipingSurfaceLines;
        }

        /// <summary>
        /// Gets the available stochastic soil models on <see cref="PipingCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<StochasticSoilModel> GetAvailableStochasticSoilModels()
        {
            if (data.WrappedData.SurfaceLine == null)
            {
                return data.AvailableStochasticSoilModels;
            }
            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(data.WrappedData.SurfaceLine, data.AvailableStochasticSoilModels);
        }

        /// <summary>
        /// Gets the available stochastic soil profiles on <see cref="PipingCalculationScenarioContext"/>.
        /// </summary>
        public IEnumerable<StochasticSoilProfile> GetAvailableStochasticSoilProfiles()
        {
            return data.WrappedData.StochasticSoilModel != null ? data.WrappedData.StochasticSoilModel.StochasticSoilProfiles : new List<StochasticSoilProfile>();
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == TypeUtils.GetMemberName<PipingInputContextProperties>(p => p.AssessmentLevel))
            {
                return !data.WrappedData.UseAssessmentLevelManualInput;
            }

            return true;
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == TypeUtils.GetMemberName<PipingInputContextProperties>(p => p.SelectedHydraulicBoundaryLocation))
            {
                return !data.WrappedData.UseAssessmentLevelManualInput;
            }

            return false;
        }

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referencePoint = SurfaceLine != null ? SurfaceLine.ReferenceLineIntersectionWorldPoint : null;

            return data.AvailableHydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, referencePoint))
                       .OrderBy(hbl => hbl.Distance)
                       .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id);
        }

        #region Hydraulic data

        [DynamicVisible]
        [PropertyOrder(selectedHydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_HydraulicBoundaryLocation_Description")]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                Point2D referencePoint = SurfaceLine != null ? SurfaceLine.ReferenceLineIntersectionWorldPoint : null;

                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation,
                                                                     referencePoint)
                           : null;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_Description")]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
            set
            {
                data.WrappedData.AssessmentLevel = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(useHydraulicBoundaryLocationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_UseAssessmentLevelManualInput_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_UseAssessmentLevelManualInput_Description")]
        public bool UseAssessmentLevelManualInput
        {
            get
            {
                return data.WrappedData.UseAssessmentLevelManualInput;
            }
            set
            {
                data.WrappedData.UseAssessmentLevelManualInput = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(dampingFactorExitPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DampingFactorExit_Description")]
        public DesignVariable<LogNormalDistribution> DampingFactorExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(data.WrappedData);
            }
            set
            {
                data.WrappedData.DampingFactorExit = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(phreaticLevelExitPropertyIndex)]
        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(data.WrappedData);
            }
            set
            {
                data.WrappedData.PhreaticLevelExit = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(piezometricHeadExitPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadExit_Description")]
        public RoundedDouble PiezometricHeadExit
        {
            get
            {
                return data.WrappedData.PiezometricHeadExit;
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(surfaceLinePropertyIndex)]
        [Editor(typeof(PipingInputContextSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SurfaceLine_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SurfaceLine_Description")]
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return data.WrappedData.SurfaceLine;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.SurfaceLine))
                {
                    data.WrappedData.SurfaceLine = value;
                    PipingInputService.SetMatchingStochasticSoilModel(data.WrappedData, GetAvailableStochasticSoilModels());
                    data.WrappedData.NotifyObservers();
                }
            }
        }

        [PropertyOrder(stochasticSoilModelPropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilModelSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_StochasticSoilModel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_StochasticSoilModel_Description")]
        public StochasticSoilModel StochasticSoilModel
        {
            get
            {
                return data.WrappedData.StochasticSoilModel;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.StochasticSoilModel))
                {
                    data.WrappedData.StochasticSoilModel = value;
                    PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(data.WrappedData);
                    data.WrappedData.NotifyObservers();
                }
            }
        }

        [PropertyOrder(stochasticSoilProfilePropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_StochasticSoilProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_StochasticSoilProfile_Description")]
        public StochasticSoilProfile StochasticSoilProfile
        {
            get
            {
                return data.WrappedData.StochasticSoilProfile;
            }
            set
            {
                if (!ReferenceEquals(value, data.WrappedData.StochasticSoilProfile))
                {
                    data.WrappedData.StochasticSoilProfile = value;
                    data.WrappedData.NotifyObservers();
                }
            }
        }

        [PropertyOrder(entryPointLPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_EntryPointL_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_EntryPointL_Description")]
        public RoundedDouble EntryPointL
        {
            get
            {
                return data.WrappedData.EntryPointL;
            }
            set
            {
                data.WrappedData.EntryPointL = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(exitPointLPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ExitPointL_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ExitPointL_Description")]
        public RoundedDouble ExitPointL
        {
            get
            {
                return data.WrappedData.ExitPointL;
            }
            set
            {
                data.WrappedData.ExitPointL = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(seepageLengthPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SeepageLength_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SeepageLength_Description")]
        public DesignVariable<LogNormalDistribution> SeepageLength
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.WrappedData);
            }
        }

        [PropertyOrder(thicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessCoverageLayer_Description")]
        public DesignVariable<LogNormalDistribution> ThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.WrappedData);
            }
        }

        [PropertyOrder(effectiveThicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_EffectiveThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_EffectiveThicknessCoverageLayer_Description")]
        public DesignVariable<LogNormalDistribution> EffectiveThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetEffectiveThicknessCoverageLayer(data.WrappedData);
            }
        }

        [PropertyOrder(thicknessAquiferLayerPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessAquiferLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessAquiferLayer_Description")]
        public DesignVariable<LogNormalDistribution> ThicknessAquiferLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.WrappedData);
            }
        }

        [PropertyOrder(darcyPermeabilityPropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DarcyPermeability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DarcyPermeability_Description")]
        public DesignVariable<LogNormalDistribution> DarcyPermeability
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.WrappedData);
            }
        }

        [PropertyOrder(diameter70PropertyIndex)]
        [TypeConverter(typeof(LogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_Diameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_Diameter70_Description")]
        public DesignVariable<LogNormalDistribution> Diameter70
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.WrappedData);
            }
        }

        [PropertyOrder(saturatedVolumicWeightOfCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ShiftedLogNormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SaturatedVolumicWeightOfCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SaturatedVolumicWeightOfCoverageLayer_Description")]
        public DesignVariable<LogNormalDistribution> SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(data.WrappedData);
            }
        }

        #endregion
    }
}