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

using Core.Common.Gui;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationContextProperties_DisplayName")]
    public class PipingCalculationContextProperties : ObjectProperties<PipingCalculationScenarioContext>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingCalculation_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingCalculation_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
            set
            {
                data.WrappedData.Name = value;
                data.WrappedData.NotifyObservers();
            }
        }
    }
}