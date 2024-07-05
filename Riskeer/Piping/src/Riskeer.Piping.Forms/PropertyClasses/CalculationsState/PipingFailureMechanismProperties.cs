﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses.CalculationsState
{
    /// <summary>
    /// Calculations state related ViewModel of <see cref="PipingFailureMechanism"/> for properties panel.
    /// </summary>
    public class PipingFailureMechanismProperties : PipingFailureMechanismPropertiesBase
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int waterVolumetricWeightPropertyIndex = 3;
        private const int upLiftModelFactorPropertyIndex = 4;
        private const int sellmeijerModelFactorPropertyIndex = 5;
        private const int criticalHeaveGradientPropertyIndex = 6;
        private const int sandParticlesVolumetricWeightPropertyIndex = 7;
        private const int whitesDragCoefficientPropertyIndex = 8;
        private const int beddingAnglePropertyIndex = 9;
        private const int waterKinematicViscosityPropertyIndex = 10;
        private const int gravityPropertyIndex = 11;
        private const int meanDiameter70PropertyIndex = 12;
        private const int sellmeijerReductionFactorPropertyIndex = 13;

        private readonly IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismProperties(PipingFailureMechanism data, IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> handler)
            : base(data, new ConstructionProperties
            {
                NamePropertyIndex = namePropertyIndex,
                CodePropertyIndex = codePropertyIndex
            })
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            propertyChangeHandler = handler;
        }

        #region Heave

        [PropertyOrder(criticalHeaveGradientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_CriticalHeaveGradient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_CriticalHeaveGradient_Description))]
        public LogNormalDistributionDesignVariableProperties CriticalHeaveGradient
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(
                    SemiProbabilisticPipingDesignVariableFactory.GetCriticalHeaveGradientDesignVariable(data.GeneralInput));
            }
        }

        #endregion

        #region General

        [PropertyOrder(waterVolumetricWeightPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WaterVolumetricWeight_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WaterVolumetricWeight_Description))]
        public RoundedDouble WaterVolumetricWeight
        {
            get
            {
                return data.GeneralInput.WaterVolumetricWeight;
            }
            set
            {
                IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                    data,
                    value,
                    (f, v) => f.GeneralInput.WaterVolumetricWeight = v);

                NotifyAffectedObjects(affectedObjects);
            }
        }

        #endregion

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #region Sellmeijer

        [PropertyOrder(sandParticlesVolumetricWeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_SandParticlesVolumicWeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_SandParticlesVolumicWeight_Description))]
        public RoundedDouble SandParticlesVolumicWeight
        {
            get
            {
                return data.GeneralInput.SandParticlesVolumicWeight;
            }
        }

        [PropertyOrder(whitesDragCoefficientPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_WhitesDragCoefficient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_WhitesDragCoefficient_Description))]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.GeneralInput.WhitesDragCoefficient;
            }
        }

        [PropertyOrder(beddingAnglePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_BeddingAngle_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_BeddingAngle_Description))]
        public double BeddingAngle
        {
            get
            {
                return data.GeneralInput.BeddingAngle;
            }
        }

        [PropertyOrder(waterKinematicViscosityPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_WaterKinematicViscosity_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_WaterKinematicViscosity_Description))]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.GeneralInput.WaterKinematicViscosity;
            }
        }

        [PropertyOrder(gravityPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.GravitationalAcceleration_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.GravitationalAcceleration_Description))]
        public double Gravity
        {
            get
            {
                return data.GeneralInput.Gravity;
            }
        }

        [PropertyOrder(meanDiameter70PropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_MeanDiameter70_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_MeanDiameter70_Description))]
        public double MeanDiameter70
        {
            get
            {
                return data.GeneralInput.MeanDiameter70;
            }
        }

        [PropertyOrder(sellmeijerReductionFactorPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerReductionFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerReductionFactor_Description))]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.GeneralInput.SellmeijerReductionFactor;
            }
        }

        #endregion

        #region Model factors

        [PropertyOrder(upLiftModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_UpliftModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_UpliftModelFactor_Description))]
        public LogNormalDistributionDesignVariableProperties UpliftModelFactor
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(
                    SemiProbabilisticPipingDesignVariableFactory.GetUpliftModelFactorDesignVariable(data.GeneralInput));
            }
        }

        [PropertyOrder(sellmeijerModelFactorPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerModelFactor_Description))]
        public LogNormalDistributionDesignVariableProperties SellmeijerModelFactor
        {
            get
            {
                return new LogNormalDistributionDesignVariableProperties(
                    SemiProbabilisticPipingDesignVariableFactory.GetSellmeijerModelFactorDesignVariable(data.GeneralInput));
            }
        }

        #endregion
    }
}