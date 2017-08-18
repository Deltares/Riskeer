// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of grid settings properties in <see cref="MacroStabilityInwardsInputContext"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsGridSettingsProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int moveGridPropertyIndex = 1;
        private const int gridDeterminationPropertyIndex = 2;
        private const int tangentLineDeterminationPropertyIndex = 3;
        private const int tangentLineZTopPropertyIndex = 4;
        private const int tangentLineZBottomPropertyIndex = 5;
        private const int leftGridPropertyIndex = 6;
        private const int rightGridPropertyIndex = 7;

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

        [PropertyOrder(gridDeterminationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.GridDetermination_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.GridDetermination_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsGridDetermination GridDetermination
        {
            get
            {
                return data.GridDetermination;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.GridDetermination = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineDeterminationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TangentLineDetermination_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.TangentLineDetermination_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MacroStabilityInwardsTangentLineDetermination TangentLineDetermination
        {
            get
            {
                return data.TangentLineDetermination;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.TangentLineDetermination = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(tangentLineZTopPropertyIndex)]
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

        [PropertyOrder(leftGridPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.GridSettings_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeftGrid_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeftGrid_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsGridProperties LeftGrid
        {
            get
            {
                return new MacroStabilityInwardsGridProperties(data.LeftGrid, propertyChangeHandler);
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
                return new MacroStabilityInwardsGridProperties(data.RightGrid, propertyChangeHandler);
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}