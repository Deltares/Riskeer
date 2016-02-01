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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using Core.Common.Gui;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingOutputPropertiesDisplayName")]
    public class PipingOutputProperties : ObjectProperties<PipingOutput>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_HeaveFactorOfSafety_Description")]
        public double HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_HeaveZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_HeaveZValue_Description")]
        public double HeaveZValue
        {
            get
            {
                return data.HeaveZValue;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_UpliftFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_UpliftFactorOfSafety_Description")]
        public double UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_UpliftZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_UpliftZValue_Description")]
        public double UpliftZValue
        {
            get
            {
                return data.UpliftZValue;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_SellmeijerFactorOfSafety_Description")]
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_SellmeijerZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_SellmeijerZValue_Description")]
        public double SellmeijerZValue
        {
            get
            {
                return data.SellmeijerZValue;
            }
        }
    }
}