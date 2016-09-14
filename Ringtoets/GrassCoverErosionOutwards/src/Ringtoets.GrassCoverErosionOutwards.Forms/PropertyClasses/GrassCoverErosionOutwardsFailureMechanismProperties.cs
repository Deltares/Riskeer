﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using log4net;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionOutwardsFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanismProperties : ObjectProperties<GrassCoverErosionOutwardsFailureMechanism>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsFailureMechanismProperties));

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
                data.GeneralInput.N = value;
                ClearHydraulicBoundaryLocationOutput();
                data.NotifyObservers();
            }
        }

        #endregion

        private void ClearHydraulicBoundaryLocationOutput()
        {
            GrassCoverErosionOutwardsDataSynchronizationService.ClearHydraulicBoundaryLocationOutput(data.HydraulicBoundaryLocations);
            data.HydraulicBoundaryLocations.NotifyObservers();
            log.Info(Resources.GrassCoverErosionOutwards_NormValueChanged_Waveheight_and_design_water_level_results_cleared);
        }

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
    }
}