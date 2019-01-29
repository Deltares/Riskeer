// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MacroStabilityInwardsGrid"/> properties for properties panel.
    /// </summary>
    public class MacroStabilityInwardsGridProperties : ObjectProperties<MacroStabilityInwardsGrid>
    {
        private const int xLeftPropertyIndex = 1;
        private const int xRightPropertyIndex = 2;
        private const int zTopPropertyIndex = 3;
        private const int zBottomPropertyIndex = 4;
        private const int numberOfHorizontalPointsPropertyIndex = 5;
        private const int numberOfVerticalPointsPropertyIndex = 6;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;
        private readonly bool isReadOnly;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsGridProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <param name="isReadOnly">Indicates whether the properties are read only.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsGridProperties(MacroStabilityInwardsGrid data, IObservablePropertyChangeHandler handler, bool isReadOnly)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.data = data;
            propertyChangeHandler = handler;
            this.isReadOnly = isReadOnly;
        }

        [PropertyOrder(xLeftPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.XLeft_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.XLeft_Description))]
        public RoundedDouble XLeft
        {
            get
            {
                return data.XLeft;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.XLeft = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(xRightPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.XRight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.XRight_Description))]
        public RoundedDouble XRight
        {
            get
            {
                return data.XRight;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.XRight = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(zTopPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZTop_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZTop_Description))]
        public RoundedDouble ZTop
        {
            get
            {
                return data.ZTop;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZTop = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(zBottomPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ZBottom_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.ZBottom_Description))]
        public RoundedDouble ZBottom
        {
            get
            {
                return data.ZBottom;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.ZBottom = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(numberOfHorizontalPointsPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NumberOfHorizontalPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.NumberOfHorizontalPoints_Description))]
        public int NumberOfHorizontalPoints
        {
            get
            {
                return data.NumberOfHorizontalPoints;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.NumberOfHorizontalPoints = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(numberOfVerticalPointsPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Grid))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.NumberOfVerticalPoints_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.NumberOfVerticalPoints_Description))]
        public int NumberOfVerticalPoints
        {
            get
            {
                return data.NumberOfVerticalPoints;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.NumberOfVerticalPoints = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return isReadOnly;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}