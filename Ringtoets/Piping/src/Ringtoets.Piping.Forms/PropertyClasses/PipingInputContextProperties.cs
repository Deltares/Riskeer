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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;
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

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        #region Hydraulic data

        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_HydraulicBoundaryLocation_Description")]
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

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_Description")]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
        }

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