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

using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsFailureMechanismContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanismContextProperties : ObjectProperties<GrassCoverErosionInwardsFailureMechanismContext>
    {
        private const int namePropertyIndex = 1;
        private const int codePropertyIndex = 2;
        private const int lengthEffectPropertyIndex = 3;
        private const int frunupModelFactorPropertyIndex = 4;
        private const int fbFactorPropertyIndex = 5;
        private const int fnFactorPropertyIndex = 6;
        private const int fshallowModelFactorPropertyIndex = 7;

        #region Lengte effect parameters

        [PropertyOrder(lengthEffectPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_LengthEffect")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_N_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_N_Description")]
        public int LengthEffect
        {
            get
            {
                return data.WrappedData.NormProbabilityInput.N;
            }
            set
            {
                data.WrappedData.NormProbabilityInput.N = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [PropertyOrder(codePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_Code_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_Code_Description")]
        public string Code
        {
            get
            {
                return data.WrappedData.Code;
            }
        }

        #endregion

        #region Model settings

        [PropertyOrder(frunupModelFactorPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FrunupModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FrunupModelFactor_Description")]
        public ReadOnlyNormalDistributionProperties FrunupModelFactor
        {
            get
            {
                return new ReadOnlyNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FrunupModelFactor
                };
            }
        }

        [PropertyOrder(fbFactorPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FbFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FbFactor_Description")]
        public ReadOnlyNormalDistributionProperties FbFactor
        {
            get
            {
                return new ReadOnlyNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FbFactor
                };
            }
        }

        [PropertyOrder(fnFactorPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FnFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FnFactor_Description")]
        public ReadOnlyNormalDistributionProperties FnFactor
        {
            get
            {
                return new ReadOnlyNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FnFactor
                };
            }
        }

        [PropertyOrder(fshallowModelFactorPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "GrassCoverErosionInwardsInput_FshallowModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GrassCoverErosionInwardsInput_FshallowModelFactor_Description")]
        public ReadOnlyNormalDistributionProperties FshallowModelFactor
        {
            get
            {
                return new ReadOnlyNormalDistributionProperties
                {
                    Data = data.WrappedData.GeneralInput.FshallowModelFactor
                };
            }
        }

        #endregion
    }
}