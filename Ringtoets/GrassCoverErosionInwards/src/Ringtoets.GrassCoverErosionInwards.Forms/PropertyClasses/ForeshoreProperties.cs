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
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ForeshoreProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        private const int useForeshorePropertyIndex = 1;
        private const int numberOfCoordinatesPropertyIndex = 2;

        [PropertyOrder(useForeshorePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "Foreshore_UseForeshore_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Foreshore_UseForeshore_Description")]
        public bool UseForeshore
        {
            get
            {
                return data.WrappedData.UseForeshore;
            }
            set
            {
                data.WrappedData.UseForeshore = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(numberOfCoordinatesPropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "Foreshore_NumberOfCoordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Foreshore_NumberOfCoordinates_Description")]
        public int NumberOfCoordinates
        {
            get
            {
                return data.WrappedData.ForeshoreGeometry.Count();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}