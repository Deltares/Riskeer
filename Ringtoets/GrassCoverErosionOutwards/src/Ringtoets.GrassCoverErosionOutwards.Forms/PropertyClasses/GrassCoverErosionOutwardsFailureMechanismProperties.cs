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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using log4net;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsRevetmentFormsResources = Ringtoets.Revetment.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionOutwardsFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismProperties : ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsFailureMechanismProperties));
        private readonly IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationsContextProperties"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to show the properties for.</param>
        /// <param name="handler">Handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsFailureMechanismProperties(
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            IGrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler changeHandler)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (changeHandler == null)
            {
                throw new ArgumentNullException("changeHandler");
            }
            Data = failureMechanism;
            this.changeHandler = changeHandler;
        }

        #region Length effect parameters

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_LengthEffect")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "FailureMechanism_N_Description")]
        public int LengthEffect
        {
            get
            {
                return data.GeneralInput.N;
            }
            set
            {
                if (changeHandler.ConfirmPropertyChange())
                {
                    data.GeneralInput.N = value;
                    ClearOutputAndNotifyObservers();
                }
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
                return data.Name;
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
                return data.Code;
            }
        }

        #endregion

        #region Model settings

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_A_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_A_Description")]
        public RoundedDouble A
        {
            get
            {
                return data.GeneralInput.GeneralWaveConditionsInput.A;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_B_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_B_Description")]
        public RoundedDouble B
        {
            get
            {
                return data.GeneralInput.GeneralWaveConditionsInput.B;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_C_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsRevetmentFormsResources), "GeneralWaveConditionsInput_C_Description")]
        public RoundedDouble C
        {
            get
            {
                return data.GeneralInput.GeneralWaveConditionsInput.C;
            }
        }

        #endregion

        private void ClearOutputAndNotifyObservers()
        {
            foreach (var observable in changeHandler.PropertyChanged(data))
            {
                observable.NotifyObservers();
            }
            data.NotifyObservers();
        }
    }
}