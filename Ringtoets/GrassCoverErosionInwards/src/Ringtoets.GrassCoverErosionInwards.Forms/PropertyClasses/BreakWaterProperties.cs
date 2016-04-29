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

using System.ComponentModel;
using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInput.BreakWater"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BreakWaterProperties : ObjectProperties<GrassCoverErosionInwardsCalculationContext>
    {
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), "BreakWater_BreakWaterPresent_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWater_BreakWaterPresent_Discription")]
        public bool BreakWaterPresent
        {
            get
            {
                return data.WrappedData.InputParameters.UseBreakWater;
            }
            set
            {
                data.WrappedData.InputParameters.UseBreakWater = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterType_Description")]
        public BreakWaterType BreakWaterType
        {
            get
            {
                var breakWater = data.WrappedData.InputParameters.BreakWater;
                return breakWater == null ? BreakWaterType.Caisson : breakWater.Type;
            }
            set
            {
                var breakWater = data.WrappedData.InputParameters.BreakWater;
                if (breakWater == null)
                {
                    return;
                }
                data.WrappedData.InputParameters.BreakWater = new BreakWater(value, breakWater.Height);
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(3)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterHeight_Description")]
        public string BreakWaterHeight
        {
            get
            {
                var breakWater = data.WrappedData.InputParameters.BreakWater;
                return breakWater == null ? string.Empty : new RoundedDouble(2, breakWater.Height).Value.ToString(CultureInfo.CurrentCulture);
            }
            set
            {
                var breakWater = data.WrappedData.InputParameters.BreakWater;
                if (breakWater == null)
                {
                    return;
                }
                data.WrappedData.InputParameters.BreakWater = new BreakWater(breakWater.Type, new RoundedDouble(2, double.Parse(value)).Value);
                data.WrappedData.NotifyObservers();
            }
        }

        public override string ToString()
        {
            return Resources.BreakWaterProperties_DisplayName;
        }
    }
}