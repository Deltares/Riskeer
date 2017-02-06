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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
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

        private readonly ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties for.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingInputContextProperties(PipingInputContext data,
            ICalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario> handler)
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

        /// <summary>
        /// Gets the available selectable hydraulic boundary locations on <see cref="PipingInputContext"/>.
        /// </summary>
        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referencePoint = SurfaceLine?.ReferenceLineIntersectionWorldPoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AvailableHydraulicBoundaryLocations, referencePoint);
        }

        #region Hydraulic data

        [DynamicVisible]
        [PropertyOrder(selectedHydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_HydraulicBoundaryLocation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_HydraulicBoundaryLocation_Description))]
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
                ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.HydraulicBoundaryLocation = v, value.HydraulicBoundaryLocation);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.AssessmentLevel_Description))]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.AssessmentLevel = v, value);
            }
        }

        [PropertyOrder(useHydraulicBoundaryLocationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_UseAssessmentLevelManualInput_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_UseAssessmentLevelManualInput_Description))]
        public bool UseAssessmentLevelManualInput
        {
            get
            {
                return data.WrappedData.UseAssessmentLevelManualInput;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.UseAssessmentLevelManualInput = v, value);
            }
        }

        [PropertyOrder(dampingFactorExitPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_DampingFactorExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_DampingFactorExit_Description))]
        public LogNormalDistributionDesignVariableProperties DampingFactorExit
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(DistributionPropertiesReadOnly.None,
                                                                         PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(data.WrappedData),
                                                                         data.PipingCalculation,
                                                                         data.WrappedData,
                                                                         propertyChangeHandler);
            }
        }

        [PropertyOrder(phreaticLevelExitPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_PhreaticLevelExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_PhreaticLevelExit_Description))]
        public NormalDistributionDesignVariableProperties PhreaticLevelExit
        {
            get
            {
                return new NormalDistributionDesignVariableProperties(DistributionPropertiesReadOnly.None,
                                                                      PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(data.WrappedData),
                                                                      data.PipingCalculation,
                                                                      data.WrappedData,
                                                                      propertyChangeHandler);
            }
        }

        [PropertyOrder(piezometricHeadExitPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_HydraulicData))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_PiezometricHeadExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_PiezometricHeadExit_Description))]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_SurfaceLine_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_SurfaceLine_Description))]
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) =>
                    {
                        input.SurfaceLine = v;
                        PipingInputService.SetMatchingStochasticSoilModel(input, GetAvailableStochasticSoilModels());
                    }, value);
                }
            }
        }

        [PropertyOrder(stochasticSoilModelPropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilModelSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilModel_Description))]
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) =>
                    {
                        input.StochasticSoilModel = v;
                        PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(input);
                    }, value);
                }
            }
        }

        [PropertyOrder(stochasticSoilProfilePropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilProfile_Description))]
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
                    ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.StochasticSoilProfile = v, value);
                }
            }
        }

        [PropertyOrder(entryPointLPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_EntryPointL_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_EntryPointL_Description))]
        public RoundedDouble EntryPointL
        {
            get
            {
                return data.WrappedData.EntryPointL;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.EntryPointL = v, value);
            }
        }

        [PropertyOrder(exitPointLPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_ExitPointL_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_ExitPointL_Description))]
        public RoundedDouble ExitPointL
        {
            get
            {
                return data.WrappedData.ExitPointL;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((input, v) => input.ExitPointL = v, value);
            }
        }

        [PropertyOrder(seepageLengthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_SeepageLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_SeepageLength_Description))]
        public LogNormalDistributionDesignVariableProperties SeepageLength
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.WrappedData));
            }
        }

        [PropertyOrder(thicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_ThicknessCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_ThicknessCoverageLayer_Description))]
        public LogNormalDistributionDesignVariableProperties ThicknessCoverageLayer
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.WrappedData));
            }
        }

        [PropertyOrder(effectiveThicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_EffectiveThicknessCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_EffectiveThicknessCoverageLayer_Description))]
        public LogNormalDistributionDesignVariableProperties EffectiveThicknessCoverageLayer
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetEffectiveThicknessCoverageLayer(data.WrappedData));
            }
        }

        [PropertyOrder(thicknessAquiferLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_ThicknessAquiferLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_ThicknessAquiferLayer_Description))]
        public LogNormalDistributionDesignVariableProperties ThicknessAquiferLayer
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.WrappedData));
            }
        }

        [PropertyOrder(darcyPermeabilityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_DarcyPermeability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_DarcyPermeability_Description))]
        public LogNormalDistributionDesignVariableProperties DarcyPermeability
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.WrappedData));
            }
        }

        [PropertyOrder(diameter70PropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_Diameter70_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_Diameter70_Description))]
        public LogNormalDistributionDesignVariableProperties Diameter70
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.WrappedData));
            }
        }

        [PropertyOrder(saturatedVolumicWeightOfCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_SaturatedVolumicWeightOfCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_SaturatedVolumicWeightOfCoverageLayer_Description))]
        public ShiftedLogNormalDistributionDesignVariableProperties SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return new ShiftedLogNormalDistributionDesignVariableProperties(
                    PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(data.WrappedData));
            }
        }


        #endregion

        private void ChangePropertyValueAndNotifyAffectedObjects<TValue>(
            SetCalculationInputPropertyValueDelegate<PipingInput, TValue> setPropertyValue,
            TValue value)
        {
            IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                data.WrappedData,
                data.PipingCalculation,
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