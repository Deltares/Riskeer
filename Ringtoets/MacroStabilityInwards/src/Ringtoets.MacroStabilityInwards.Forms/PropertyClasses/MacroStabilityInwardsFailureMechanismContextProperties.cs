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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsFailureMechanismContext"/> properties panel.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismContextProperties : ObjectProperties<MacroStabilityInwardsFailureMechanismContext>
    {
        private readonly IFailureMechanismPropertyChangeHandler<MacroStabilityInwardsFailureMechanism> propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismContextProperties"/>.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismContextProperties(
            MacroStabilityInwardsFailureMechanismContext data,
            IFailureMechanismPropertyChangeHandler<MacroStabilityInwardsFailureMechanism> handler)
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

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (!data.WrappedData.IsRelevant && ShouldHidePropertyWhenFailureMechanismIrrelevant(propertyName))
            {
                return false;
            }
            return true;
        }

        private void ChangePropertyValueAndNotifyAffectedObjects<TValue>(
            SetFailureMechanismPropertyValueDelegate<MacroStabilityInwardsFailureMechanism, TValue> setPropertyValue,
            TValue value)
        {
            IEnumerable<IObservable> affectedObjects = propertyChangeHandler.SetPropertyValueAfterConfirmation(
                data.WrappedData,
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
            return nameof(A).Equals(propertyName)
                   || nameof(B).Equals(propertyName);
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
                return data.WrappedData.Name;
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
                return data.WrappedData.Code;
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
                return data.WrappedData.IsRelevant;
            }
        }

        #endregion

        #region Semi-probabilistic parameters

        [DynamicVisible]
        [PropertyOrder(21)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_SemiProbabilisticParameters))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralMacroStabilityInwardsInput_A_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralMacroStabilityInwardsInput_A_Description))]
        public double A
        {
            get
            {
                return data.WrappedData.MacroStabilityInwardsProbabilityAssessmentInput.A;
            }
            set
            {
                ChangePropertyValueAndNotifyAffectedObjects((f, v) => f.MacroStabilityInwardsProbabilityAssessmentInput.A = v, value);
            }
        }

        [DynamicVisible]
        [PropertyOrder(22)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_SemiProbabilisticParameters))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GeneralMacroStabilityInwardsInput_B_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GeneralMacroStabilityInwardsInput_B_Description))]
        public double B
        {
            get
            {
                return data.WrappedData.MacroStabilityInwardsProbabilityAssessmentInput.B;
            }
        }

        #endregion
    }
}