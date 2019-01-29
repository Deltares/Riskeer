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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of grid settings properties in <see cref="MacroStabilityInwardsInputContext"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsGridSettingsProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int moveGridPropertyIndex = 1;
        private const int gridDeterminationTypePropertyIndex = 2;
        private const int tangentLineDeterminationTypePropertyIndex = 3;
        private const int tangentLineZTopPropertyIndex = 4;
        private const int tangentLineZBottomPropertyIndex = 5;
        private const int tangentLineNumberPropertyIndex = 6;
        private const int leftGridPropertyIndex = 7;
        private const int rightGridPropertyIndex = 8;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsGridSettingsProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsGridSettingsProperties(MacroStabilityInwardsInput data, IObservablePropertyChangeHandler handler)
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
        }

        [PropertyOrder(moveGridPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MoveGrid_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MoveGrid_Description))]
        public bool MoveGrid
        {
            get
            {
                return data.MoveGrid;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.MoveGrid = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(gridDeterminationTypePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GridDeterminationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GridDeterminationType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsGridDeterminationType GridDeterminationType
        {
            get
            {
                return data.GridDeterminationType;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.GridDeterminationType = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineDeterminationTypePropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TangentLineDeterminationType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TangentLineDeterminationType_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsTangentLineDeterminationType TangentLineDeterminationType
        {
            get
            {
                return data.TangentLineDeterminationType;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TangentLineDeterminationType = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineZTopPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TangentLineZTop_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TangentLineZTop_Description))]
        public RoundedDouble TangentLineZTop
        {
            get
            {
                return data.TangentLineZTop;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TangentLineZTop = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineZBottomPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TangentLineZBottom_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TangentLineZBottom_Description))]
        public RoundedDouble TangentLineZBottom
        {
            get
            {
                return data.TangentLineZBottom;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TangentLineZBottom = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineNumberPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TangentLineNumber_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TangentLineNumber_Description))]
        public int TangentLineNumber
        {
            get
            {
                return data.TangentLineNumber;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TangentLineNumber = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(leftGridPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeftGrid_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeftGrid_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsGridProperties LeftGrid
        {
            get
            {
                return new MacroStabilityInwardsGridProperties(data.LeftGrid, propertyChangeHandler, AreGridSettingsReadOnly());
            }
        }

        [PropertyOrder(rightGridPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.RightGrid_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.RightGrid_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsGridProperties RightGrid
        {
            get
            {
                return new MacroStabilityInwardsGridProperties(data.RightGrid, propertyChangeHandler, AreGridSettingsReadOnly());
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(TangentLineZTop) || propertyName == nameof(TangentLineZBottom) || propertyName == nameof(TangentLineNumber))
            {
                return data.TangentLineDeterminationType == MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated
                       || AreGridSettingsReadOnly();
            }

            return AreGridSettingsReadOnly();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        private bool AreGridSettingsReadOnly()
        {
            return data.GridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic;
        }
    }
}