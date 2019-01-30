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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanism"/> properties panel.
    /// </summary>
    public class PipingFailureMechanismProperties : ObjectProperties<PipingFailureMechanism>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int groupPropertyIndex = 3;
        private const int contributionPropertyIndex = 4;
        private const int isRelevantPropertyIndex = 5;
        private const int waterVolumetricWeightPropertyIndex = 6;
        private const int upLiftModelFactorPropertyIndex = 7;
        private const int sellMeijerModelFactorPropertyIndex = 8;
        private const int aPropertyIndex = 9;
        private const int bPropertyIndex = 10;
        private const int sectionLengthPropertyIndex = 11;
        private const int nPropertyIndex = 12;
        private const int criticalHeaveGradientPropertyIndex = 13;
        private const int sandParticlesVolumetricWeightPropertyIndex = 14;
        private const int whitesDragCoefficientPropertyIndex = 15;
        private const int beddingAnglePropertyIndex = 16;
        private const int waterKinematicViscosityPropertyIndex = 17;
        private const int gravityPropertyIndex = 18;
        private const int meanDiameter70PropertyIndex = 19;
        private const int sellMeijerReductionFactorPropertyIndex = 20;

        private readonly IAssessmentSection assessmentSection;
        private readonly IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="assessmentSection">The assessment section the data belongs to.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismProperties(PipingFailureMechanism data,
                                                IAssessmentSection assessmentSection,
                                                IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> handler)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Data = data;
            this.assessmentSection = assessmentSection;
            propertyChangeHandler = handler;
        }

        #region Heave

        [DynamicVisible]
        [PropertyOrder(criticalHeaveGradientPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Heave))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_CriticalHeaveGradient_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_CriticalHeaveGradient_Description))]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.GeneralInput.CriticalHeaveGradient;
            }
        }

        #endregion

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (!data.IsRelevant && ShouldHidePropertyWhenFailureMechanismIrrelevant(propertyName))
            {
                return false;
            }

            return true;
        }

        private void ChangePropertyValueAndNotifyAffectedObjects<TValue>(
            SetFailureMechanismPropertyValueDelegate<PipingFailureMechanism, TValue> setPropertyValue,
            TValue value)
        {
            IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                data,
                value,
                setPropertyValue);

            NotifyAffectedObjects(affectedObjects);
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        private bool ShouldHidePropertyWhenFailureMechanismIrrelevant(string propertyName)
        {
            return nameof(Contribution).Equals(propertyName)
                   || nameof(CriticalHeaveGradient).Equals(propertyName)
                   || nameof(WaterVolumetricWeight).Equals(propertyName)
                   || nameof(A).Equals(propertyName)
                   || nameof(B).Equals(propertyName)
                   || nameof(SectionLength).Equals(propertyName)
                   || nameof(N).Equals(propertyName)
                   || nameof(SandParticlesVolumicWeight).Equals(propertyName)
                   || nameof(WhitesDragCoefficient).Equals(propertyName)
                   || nameof(BeddingAngle).Equals(propertyName)
                   || nameof(WaterKinematicViscosity).Equals(propertyName)
                   || nameof(Gravity).Equals(propertyName)
                   || nameof(MeanDiameter70).Equals(propertyName)
                   || nameof(SellmeijerReductionFactor).Equals(propertyName)
                   || nameof(UpliftModelFactor).Equals(propertyName)
                   || nameof(SellmeijerModelFactor).Equals(propertyName);
        }

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [PropertyOrder(groupPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Group_Description))]
        public int Group
        {
            get
            {
                return data.Group;
            }
        }

        [DynamicVisible]
        [PropertyOrder(contributionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_Contribution_Description))]
        public double Contribution
        {
            get
            {
                return data.Contribution;
            }
        }

        [PropertyOrder(isRelevantPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_IsRelevant_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_IsRelevant_Description))]
        public bool IsRelevant
        {
            get
            {
                return data.IsRelevant;
            }
        }

        [DynamicVisible]
        [PropertyOrder(waterVolumetricWeightPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_WaterVolumetricWeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_WaterVolumetricWeight_Description))]
        public RoundedDouble WaterVolumetricWeight
        {
            get
            {
                return data.GeneralInput.WaterVolumetricWeight;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((f, v) => f.GeneralInput.WaterVolumetricWeight = v, value);
            }
        }

        #endregion

        #region Length effect parameters

        [DynamicVisible]
        [PropertyOrder(aPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_Description))]
        public double A
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.A;
            }
            set
            {
                data.PipingProbabilityAssessmentInput.A = value;
                data.NotifyObservers();
            }
        }

        [DynamicVisible]
        [PropertyOrder(bPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_Description))]
        public double B
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.B;
            }
        }

        [DynamicVisible]
        [PropertyOrder(sectionLengthPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ReferenceLine_Length_Rounded_Description))]
        public RoundedDouble SectionLength
        {
            get
            {
                return new RoundedDouble(2, assessmentSection.ReferenceLine.Length);
            }
        }

        [DynamicVisible]
        [PropertyOrder(nPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.FailureMechanism_N_Rounded_Description))]
        public RoundedDouble N
        {
            get
            {
                PipingProbabilityAssessmentInput probabilityAssessmentInput = data.PipingProbabilityAssessmentInput;
                return new RoundedDouble(2, probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length));
            }
        }

        #endregion

        #region Sellmeijer

        [DynamicVisible]
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

        [DynamicVisible]
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

        [DynamicVisible]
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

        [DynamicVisible]
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

        [DynamicVisible]
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

        [DynamicVisible]
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

        [DynamicVisible]
        [PropertyOrder(sellMeijerReductionFactorPropertyIndex)]
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

        [DynamicVisible]
        [PropertyOrder(upLiftModelFactorPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_UpliftModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_UpliftModelFactor_Description))]
        public double UpliftModelFactor
        {
            get
            {
                return data.GeneralInput.UpliftModelFactor;
            }
        }

        [DynamicVisible]
        [PropertyOrder(sellMeijerModelFactorPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelSettings))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerModelFactor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralPipingInput_SellmeijerModelFactor_Description))]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.GeneralInput.SellmeijerModelFactor;
            }
        }

        #endregion
    }
}