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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanismContext"/> properties panel.
    /// </summary>
    public class PipingFailureMechanismContextProperties : ObjectProperties<PipingFailureMechanismContext>
    {
        #region Heave

        [PropertyOrder(31)]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.WrappedData.GeneralInput.CriticalHeaveGradient;
            }
        }

        #endregion

        #region General

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Code_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_Code_Description")]
        public string Code
        {
            get
            {
                return data.WrappedData.Code;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_Description")]
        public RoundedDouble WaterVolumetricWeight
        {
            get
            {
                return data.WrappedData.GeneralInput.WaterVolumetricWeight;
            }
            set
            {
                data.WrappedData.GeneralInput.WaterVolumetricWeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Model factors

        [PropertyOrder(11)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.UpliftModelFactor;
            }
        }

        [PropertyOrder(12)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.SellmeijerModelFactor;
            }
        }

        #endregion

        #region Semi-probabilistic parameters

        [PropertyOrder(21)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_A_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_A_Description")]
        public double A
        {
            get
            {
                return data.WrappedData.PipingProbabilityAssessmentInput.A;
            }
            set
            {
                data.WrappedData.PipingProbabilityAssessmentInput.A = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(22)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_B_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_B_Description")]
        public double B
        {
            get
            {
                return data.WrappedData.PipingProbabilityAssessmentInput.B;
            }
        }

        #endregion

        #region Sellmeijer

        [PropertyOrder(51)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_Description")]
        public RoundedDouble SandParticlesVolumicWeight
        {
            get
            {
                return data.WrappedData.GeneralInput.SandParticlesVolumicWeight;
            }
        }

        [PropertyOrder(52)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WrappedData.GeneralInput.WhitesDragCoefficient;
            }
        }

        [PropertyOrder(53)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.WrappedData.GeneralInput.BeddingAngle;
            }
        }

        [PropertyOrder(54)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WrappedData.GeneralInput.WaterKinematicViscosity;
            }
        }

        [PropertyOrder(55)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "GravitationalAcceleration_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "GravitationalAcceleration_Description")]
        public double Gravity
        {
            get
            {
                return data.WrappedData.GeneralInput.Gravity;
            }
        }

        [PropertyOrder(56)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.WrappedData.GeneralInput.MeanDiameter70;
            }
        }

        [PropertyOrder(57)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.SellmeijerReductionFactor;
            }
        }

        #endregion
    }
}