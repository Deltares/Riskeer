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
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StochasticSoilProfile"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfileProperties_DisplayName")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StochasticSoilProfileProperties : ObjectProperties<StochasticSoilProfile>
    {
        /// <summary>
        /// Gets the name from the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfile_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilProfile_Name_Description")]
        public string Name
        {
            get
            {
                return (data.SoilProfile != null) ? data.SoilProfile.Name : String.Empty;
            }
        }

        /// <summary>
        /// Gets the probability from the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfile_Probability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilProfile_Probability_Description")]
        public double Probability
        {
            get
            {
                return data.Probability*100;
            }
        }

        /// <summary>
        /// Gets the top levels from the <see cref="Ringtoets.Piping.Primitives.PipingSoilProfile"/> 
        /// in the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        [PropertyOrder(3)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfile_Tops_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilProfile_Tops_Description")]
        public double[] TopLevels
        {
            get
            {
                return (data.SoilProfile != null) ? data.SoilProfile.Layers.Select(l => l.Top).ToArray() : new double[0];
            }
        }

        /// <summary>
        /// Gets the bottom from the <see cref="Ringtoets.Piping.Primitives.PipingSoilProfile"/> 
        /// in the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        [PropertyOrder(4)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfile_Bottom_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilProfile_Bottom_Description")]
        public double Bottom
        {
            get
            {
                return (data.SoilProfile != null) ? data.SoilProfile.Bottom : double.NaN;
            }
        }

        /// <summary>
        /// Gets the type from the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        [PropertyOrder(5)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "StochasticSoilProfile_Type_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StochasticSoilProfile_Type_Description")]
        public string Type
        {
            get
            {
                return (data.SoilProfileType == SoilProfileType.SoilProfile1D) ? "1D profiel" : "2D profiel";
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}