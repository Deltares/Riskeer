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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingInputContextProperties : ObjectProperties<PipingInputContext>
    {
        /// <summary>
        /// Gets the available surface lines on <see cref="PipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailablePipingSurfaceLines;
        }

        /// <summary>
        /// Gets the available soil profiles on <see cref="PipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> GetAvailableSoilProfiles()
        {
            return data.AvailablePipingSoilProfiles;
        }

        /// <summary>
        /// Gets the available hydraulic boundary locations on <see cref="PipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        private double WaterVolumetricWeight
        {
            get
            {
                return data.WrappedData.WaterVolumetricWeight;
            }
            set
            {
                data.WrappedData.WaterVolumetricWeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        private double PiezometricHeadPolder
        {
            get
            {
                return data.WrappedData.PiezometricHeadPolder;
            }
            set
            {
                data.WrappedData.PiezometricHeadPolder = value;
                data.WrappedData.NotifyObservers();
            }
        }

        private double AssessmentLevel
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

        private double PiezometricHeadExit
        {
            get
            {
                return data.WrappedData.PiezometricHeadExit;
            }
            set
            {
                data.WrappedData.PiezometricHeadExit = value;
                data.WrappedData.NotifyObservers();
            }
        }

        private DesignVariable<LognormalDistribution> DampingFactorExit
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

        private DesignVariable<NormalDistribution> PhreaticLevelExit
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

        private DesignVariable<LognormalDistribution> ThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.WrappedData);
            }
            set
            {
                data.WrappedData.ThicknessCoverageLayer = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        #region General

        [Editor(typeof(PipingInputContextSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
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
                data.WrappedData.SetSurfaceLine(value);
                data.WrappedData.NotifyObservers();
            }
        }

        [Editor(typeof(PipingInputContextSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SoilProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SoilProfile_Description")]
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return data.WrappedData.SoilProfile;
            }
            set
            {
                data.WrappedData.SoilProfile = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [Editor(typeof(PipingInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
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
                try
                {
                    data.WrappedData.AssessmentLevel = value.DesignWaterLevel;
                }
                catch (ArgumentException e)
                {
                    string message = string.Format(Resources.PipingInputContextProperties_HydraulicBoundaryLocation_Could_not_set_Location_0_Cause_1_, 
                        value.Name,
                        e.Message);
                    throw new ArgumentException(message);
                }
                data.WrappedData.HydraulicBoundaryLocation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Model Factors

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.WrappedData.UpliftModelFactor;
            }
            set
            {
                data.WrappedData.UpliftModelFactor = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.WrappedData.SellmeijerModelFactor;
            }
            set
            {
                data.WrappedData.SellmeijerModelFactor = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Heave

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.WrappedData.CriticalHeaveGradient;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadExit_Description")]
        public double PiezometricHeadExitHeave
        {
            get
            {
                return PiezometricHeadExit;
            }
            set
            {
                PiezometricHeadExit = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessCoverageLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessCoverageLayerHeave
        {
            get
            {
                return ThicknessCoverageLayer;
            }
            set
            {
                ThicknessCoverageLayer = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadPolder_Description")]
        public double PiezometricHeadPolderHeave
        {
            get
            {
                return PiezometricHeadPolder;
            }
            set
            {
                PiezometricHeadPolder = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DampingFactorExit_Description")]
        public DesignVariable<LognormalDistribution> DampingFactorExitHeave
        {
            get
            {
                return DampingFactorExit;
            }
            set
            {
                DampingFactorExit = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitHeave
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        #endregion

        #region Uplift 

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_EntryPointL_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_EntryPointL_Description")]
        public double EntryPointL
        {
            get
            {
                return data.WrappedData.ExitPointL - data.WrappedData.SeepageLength.Mean;
            }
            set
            {
                data.WrappedData.SetEntryPointL(value);
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ExitPointL_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ExitPointL_Description")]
        public double ExitPointL
        {
            get
            {
                return data.WrappedData.ExitPointL;
            }
            set
            {
                data.WrappedData.SetExitPointL(value);
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeightUplift
        {
            get
            {
                return WaterVolumetricWeight;
            }
            set
            {
                WaterVolumetricWeight = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_AssessmentLevel_Description")]
        public double AssessmentLevelUplift
        {
            get
            {
                return AssessmentLevel;
            }
            set
            {
                AssessmentLevel = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadExit_Description")]
        public double PiezometricHeadExitUplift
        {
            get
            {
                return PiezometricHeadExit;
            }
            set
            {
                PiezometricHeadExit = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DampingFactorExit_Description")]
        public DesignVariable<LognormalDistribution> DampingFactorExitUplift
        {
            get
            {
                return DampingFactorExit;
            }
            set
            {
                DampingFactorExit = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitUplift
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadPolder_Description")]
        public double PiezometricHeadPolderUplift
        {
            get
            {
                return PiezometricHeadPolder;
            }
            set
            {
                PiezometricHeadPolder = value;
            }
        }

        #endregion

        #region Sellmeijer

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_AssessmentLevel_Description")]
        public double AssessmentLevelSellmeijer
        {
            get
            {
                return AssessmentLevel;
            }
            set
            {
                AssessmentLevel = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitSellmeijer
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.WrappedData.SellmeijerReductionFactor;
            }
            set
            {
                data.WrappedData.SellmeijerReductionFactor = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessCoverageLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessCoverageLayerSellmeijer
        {
            get
            {
                return ThicknessCoverageLayer;
            }
            set
            {
                ThicknessCoverageLayer = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SeepageLength_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SeepageLength_Description")]
        public DesignVariable<LognormalDistribution> SeepageLength
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.WrappedData);
            }
            set
            {
                data.WrappedData.SeepageLength = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.WrappedData.SandParticlesVolumicWeight;
            }
            set
            {
                data.WrappedData.SandParticlesVolumicWeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WrappedData.WhitesDragCoefficient;
            }
            set
            {
                data.WrappedData.WhitesDragCoefficient = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_Diameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_Diameter70_Description")]
        public DesignVariable<LognormalDistribution> Diameter70
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.WrappedData);
            }
            set
            {
                data.WrappedData.Diameter70 = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeightSellmeijer
        {
            get
            {
                return WaterVolumetricWeight;
            }
            set
            {
                WaterVolumetricWeight = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DarcyPermeability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DarcyPermeability_Description")]
        public DesignVariable<LognormalDistribution> DarcyPermeability
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.WrappedData);
            }
            set
            {
                data.WrappedData.DarcyPermeability = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WrappedData.WaterKinematicViscosity;
            }
            set
            {
                data.WrappedData.WaterKinematicViscosity = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.WrappedData.Gravity;
            }
            set
            {
                data.WrappedData.Gravity = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessAquiferLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessAquiferLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessAquiferLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.WrappedData);
            }
            set
            {
                data.WrappedData.ThicknessAquiferLayer = value.Distribution;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.WrappedData.MeanDiameter70;
            }
            set
            {
                data.WrappedData.MeanDiameter70 = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.WrappedData.BeddingAngle;
            }
            set
            {
                data.WrappedData.BeddingAngle = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion
    }
}