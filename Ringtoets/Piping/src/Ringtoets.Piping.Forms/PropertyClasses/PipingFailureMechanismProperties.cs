// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanism"/> properties panel.
    /// </summary>
    public class PipingFailureMechanismProperties : ObjectProperties<PipingFailureMechanism>
    {
        private readonly IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public PipingFailureMechanismProperties(PipingFailureMechanism data,
                                                IFailureMechanismPropertyChangeHandler<PipingFailureMechanism> handler)
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

        #region Heave

        [DynamicVisible]
        [PropertyOrder(31)]
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
            return nameof(CriticalHeaveGradient).Equals(propertyName)
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

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Name_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Code_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_Code_Description))]
        public string Code
        {
            get
            {
                return data.Code;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_IsRelevant_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_IsRelevant_Description))]
        public bool IsRelevant
        {
            get
            {
                return data.IsRelevant;
            }
        }

        [DynamicVisible]
        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
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

        #region Semi-probabilistic parameters

        [DynamicVisible]
        [PropertyOrder(21)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_A_Description))]
        public double A
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.A;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((f, v) => f.PipingProbabilityAssessmentInput.A = v, value);
            }
        }

        [DynamicVisible]
        [PropertyOrder(22)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_ProbabilityAssessmentInput_B_Description))]
        public double B
        {
            get
            {
                return data.PipingProbabilityAssessmentInput.B;
            }
        }

        [DynamicVisible]
        [PropertyOrder(23)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ReferenceLine_Length_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.ReferenceLine_Length_Description))]
        public RoundedDouble SectionLength
        {
            get
            {
                return new RoundedDouble(2, data.PipingProbabilityAssessmentInput.SectionLength);
            }
        }

        [DynamicVisible]
        [PropertyOrder(24)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_LengthEffect))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_Rounded_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.FailureMechanism_N_Rounded_Description))]
        public RoundedDouble N
        {
            get
            {
                return new RoundedDouble(2, data.PipingProbabilityAssessmentInput.GetSectionSpecificN(
                                             data.PipingProbabilityAssessmentInput.SectionLength));
            }
        }

        #endregion

        #region Sellmeijer

        [DynamicVisible]
        [PropertyOrder(51)]
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
        [PropertyOrder(52)]
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
        [PropertyOrder(53)]
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
        [PropertyOrder(54)]
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
        [PropertyOrder(55)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Sellmeijer))]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.GravitationalAcceleration_DisplayName))]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.GravitationalAcceleration_Description))]
        public double Gravity
        {
            get
            {
                return data.GeneralInput.Gravity;
            }
        }

        [DynamicVisible]
        [PropertyOrder(56)]
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
        [PropertyOrder(57)]
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
        [PropertyOrder(11)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
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
        [PropertyOrder(12)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_ModelSettings))]
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