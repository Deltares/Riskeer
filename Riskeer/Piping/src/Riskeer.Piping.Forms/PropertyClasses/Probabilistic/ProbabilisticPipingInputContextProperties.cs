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
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.UITypeEditors;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.Properties;
using Riskeer.Piping.Forms.UITypeEditors;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.Probabilistic
{
    /// <summary>
    /// ViewModel of <see cref="ProbabilisticPipingInputContext"/> for properties panel.
    /// </summary>
    public class ProbabilisticPipingInputContextProperties : ObjectProperties<ProbabilisticPipingInputContext>,
                                                             IHasHydraulicBoundaryLocationProperty,
                                                             IHasSurfaceLineProperty,
                                                             IHasStochasticSoilModel,
                                                             IHasStochasticSoilProfile
    {
        private const int selectedHydraulicBoundaryLocationPropertyIndex = 1;
        private const int dampingFactorExitPropertyIndex = 2;
        private const int phreaticLevelExitPropertyIndex = 3;
        private const int piezometricHeadExitPropertyIndex = 4;

        private const int surfaceLinePropertyIndex = 5;
        private const int stochasticSoilModelPropertyIndex = 6;
        private const int stochasticSoilProfilePropertyIndex = 7;
        private const int entryPointLPropertyIndex = 8;
        private const int exitPointLPropertyIndex = 9;
        private const int seepageLengthPropertyIndex = 10;
        private const int thicknessCoverageLayerPropertyIndex = 11;
        private const int effectiveThicknessCoverageLayerPropertyIndex = 12;
        private const int thicknessAquiferLayerPropertyIndex = 13;
        private const int darcyPermeabilityPropertyIndex = 14;
        private const int diameter70PropertyIndex = 15;
        private const int saturatedVolumicWeightOfCoverageLayerPropertyIndex = 16;

        private const int sectionNamePropertyIndex = 17;
        private const int sectionLengthPropertyIndex = 18;

        private const int shouldIllustrationPointsBeCalculatedPropertyIndex = 19;

        private readonly Func<RoundedDouble> getNormativeAssessmentLevelFunc;
        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingInputContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties for.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/> for obtaining the normative assessment level.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ProbabilisticPipingInputContextProperties(ProbabilisticPipingInputContext data,
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
                throw new ArgumentNullException(nameof(propertyChangeHandler));
            }

            Data = data;

            this.getNormativeAssessmentLevelFunc = getNormativeAssessmentLevelFunc;
            this.propertyChangeHandler = propertyChangeHandler;
        }

        #region Output

        [PropertyOrder(shouldIllustrationPointsBeCalculatedPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_OutputSettings), 4, 4)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ShouldIllustrationPointsBeCalculated_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ShouldIllustrationPointsBeCalculated_Description))]
        public bool ShouldIllustrationPointsBeCalculated
        {
            get
            {
                return data.WrappedData.ShouldIllustrationPointsBeCalculated;
            }
            set
            {
                data.WrappedData.ShouldIllustrationPointsBeCalculated = value;
                data.NotifyObservers();
            }
        }

        #endregion

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(EntryPointL) || propertyName == nameof(ExitPointL))
            {
                return data.WrappedData.SurfaceLine == null;
            }

            return true;
        }

        /// <summary>
        /// Gets the available selectable hydraulic boundary locations on <see cref="ProbabilisticPipingInputContext"/>.
        /// </summary>
        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            Point2D referencePoint = SurfaceLine?.ReferenceLineIntersectionWorldPoint;
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AssessmentSection.HydraulicBoundaryDatabase.Locations, referencePoint);
        }

        /// <summary>
        /// Gets the available stochastic soil models on <see cref="ProbabilisticPipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<PipingStochasticSoilModel> GetAvailableStochasticSoilModels()
        {
            if (data.WrappedData.SurfaceLine == null)
            {
                return data.AvailableStochasticSoilModels;
            }

            return PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(data.WrappedData.SurfaceLine,
                                                                                              data.AvailableStochasticSoilModels);
        }

        /// <summary>
        /// Gets the available stochastic soil profiles on <see cref="ProbabilisticPipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<PipingStochasticSoilProfile> GetAvailableStochasticSoilProfiles()
        {
            return data.WrappedData.StochasticSoilModel != null
                       ? data.WrappedData.StochasticSoilModel.StochasticSoilProfiles
                       : new List<PipingStochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the available surface lines on <see cref="ProbabilisticPipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailablePipingSurfaceLines;
        }

        private FailureMechanismSection GetSection()
        {
            return data.FailureMechanism
                       .Sections
                       .FirstOrDefault(section => data.PipingCalculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Math2D.ConvertPointsToLineSegments(section.Points)));
        }

        #region Hydraulic data

        [PropertyOrder(selectedHydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), 1, 4)]
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

        [PropertyOrder(dampingFactorExitPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_DampingFactorExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_DampingFactorExit_Description))]
        public LogNormalDistributionProperties DampingFactorExit
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionReadOnlyProperties.None,
                                                           data.WrappedData.DampingFactorExit,
                                                           propertyChangeHandler);
            }
        }

        [PropertyOrder(phreaticLevelExitPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_PhreaticLevelExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_PhreaticLevelExit_Description))]
        public NormalDistributionProperties PhreaticLevelExit
        {
            get
            {
                return new NormalDistributionProperties(DistributionReadOnlyProperties.None,
                                                        data.WrappedData.PhreaticLevelExit,
                                                        propertyChangeHandler);
            }
        }

        [PropertyOrder(piezometricHeadExitPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_HydraulicData), 1, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_PiezometricHeadExit_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_PiezometricHeadExit_Description))]
        public RoundedDouble PiezometricHeadExit
        {
            get
            {
                return SemiProbabilisticDerivedPipingInput.GetPiezometricHeadExit(data.WrappedData, getNormativeAssessmentLevelFunc());
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(surfaceLinePropertyIndex)]
        [Editor(typeof(PipingInputContextSurfaceLineSelectionEditor<ProbabilisticPipingInputContextProperties>), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.SurfaceLine_Description))]
        public PipingSurfaceLine SurfaceLine
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
                        PipingInputService.SetMatchingStochasticSoilModel(data.WrappedData, GetAvailableStochasticSoilModels());
                    }, propertyChangeHandler);
                }
            }
        }

        [PropertyOrder(stochasticSoilModelPropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilModelSelectionEditor<ProbabilisticPipingInputContextProperties>), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilModel_Description))]
        public PipingStochasticSoilModel StochasticSoilModel
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
                        PipingInputService.SyncStochasticSoilProfileWithStochasticSoilModel(data.WrappedData);
                    }, propertyChangeHandler);
                }
            }
        }

        [PropertyOrder(stochasticSoilProfilePropertyIndex)]
        [Editor(typeof(PipingInputContextStochasticSoilProfileSelectionEditor<ProbabilisticPipingInputContextProperties>), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilProfile_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_StochasticSoilProfile_Description))]
        public PipingStochasticSoilProfile StochasticSoilProfile
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

        [DynamicReadOnly]
        [PropertyOrder(entryPointLPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.EntryPointL = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(exitPointLPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
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
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WrappedData.ExitPointL = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(seepageLengthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_SeepageLength_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_SeepageLength_Description))]
        public VariationCoefficientLogNormalDistributionProperties SeepageLength
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(DerivedPipingInput.GetSeepageLength(data.WrappedData));
            }
        }

        [PropertyOrder(thicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_ThicknessCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_ThicknessCoverageLayer_Description))]
        public LogNormalDistributionProperties ThicknessCoverageLayer
        {
            get
            {
                return new LogNormalDistributionProperties(DerivedPipingInput.GetThicknessCoverageLayer(data.WrappedData));
            }
        }

        [PropertyOrder(effectiveThicknessCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_EffectiveThicknessCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_EffectiveThicknessCoverageLayer_Description))]
        public LogNormalDistributionProperties EffectiveThicknessCoverageLayer
        {
            get
            {
                return new LogNormalDistributionProperties(DerivedPipingInput.GetEffectiveThicknessCoverageLayer(data.WrappedData));
            }
        }

        [PropertyOrder(thicknessAquiferLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_ThicknessAquiferLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_ThicknessAquiferLayer_Description))]
        public LogNormalDistributionProperties ThicknessAquiferLayer
        {
            get
            {
                return new LogNormalDistributionProperties(DerivedPipingInput.GetThicknessAquiferLayer(data.WrappedData));
            }
        }

        [PropertyOrder(darcyPermeabilityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_DarcyPermeability_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_DarcyPermeability_Description))]
        public VariationCoefficientLogNormalDistributionProperties DarcyPermeability
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(DerivedPipingInput.GetDarcyPermeability(data.WrappedData));
            }
        }

        [PropertyOrder(diameter70PropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_Diameter70_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_Diameter70_Description))]
        public VariationCoefficientLogNormalDistributionProperties Diameter70
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(DerivedPipingInput.GetDiameterD70(data.WrappedData));
            }
        }

        [PropertyOrder(saturatedVolumicWeightOfCoverageLayerPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization), 2, 4)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PipingInput_SaturatedVolumicWeightOfCoverageLayer_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PipingInput_SaturatedVolumicWeightOfCoverageLayer_Description))]
        public ShiftedLogNormalDistributionProperties SaturatedVolumicWeightOfCoverageLayer
        {
            get
            {
                return new ShiftedLogNormalDistributionProperties(DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(data.WrappedData));
            }
        }

        #endregion

        #region Section information

        [PropertyOrder(sectionNamePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_FailureMechanismSection), 3, 4)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSection_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSection_Name_Description))]
        public string SectionName
        {
            get
            {
                FailureMechanismSection failureMechanismSection = GetSection();
                return failureMechanismSection == null ? "-" : failureMechanismSection.Name;
            }
        }

        [PropertyOrder(sectionLengthPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_FailureMechanismSection), 3, 4)]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSection_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanismSection_Length_Rounded_Description))]
        public double SectionLength
        {
            get
            {
                FailureMechanismSection failureMechanismSection = GetSection();
                return failureMechanismSection == null ? 0.00d : failureMechanismSection.Length;
            }
        }

        #endregion
    }
}