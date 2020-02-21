// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of offset properties in <see cref="MacroStabilityInwardsLocationInputBase"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsLocationInputOffsetProperties : ObjectProperties<MacroStabilityInwardsLocationInputBase>
    {
        private const int useDefaultOffsetsPropertyIndex = 1;
        private const int phreaticLineOffsetBelowDikeTopAtRiverPropertyIndex = 2;
        private const int phreaticLineOffsetBelowDikeTopAtPolderPropertyIndex = 3;
        private const int phreaticLineOffsetBelowShoulderBaseInsidePropertyIndex = 4;
        private const int phreaticLineOffsetBelowDikeToeAtPolderPropertyIndex = 5;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsLocationInputOffsetProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsLocationInputOffsetProperties(MacroStabilityInwardsLocationInputBase data, IObservablePropertyChangeHandler handler)
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

        [PropertyOrder(useDefaultOffsetsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.UseDefaultOffsets_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.UseDefaultOffsets_Description))]
        public bool UseDefaultOffsets
        {
            get
            {
                return data.UseDefaultOffsets;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.UseDefaultOffsets = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(phreaticLineOffsetBelowDikeTopAtRiverPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeTopAtRiver_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeTopAtRiver_Description))]
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtRiver
        {
            get
            {
                return data.PhreaticLineOffsetBelowDikeTopAtRiver;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PhreaticLineOffsetBelowDikeTopAtRiver = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(phreaticLineOffsetBelowDikeTopAtPolderPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeTopAtPolder_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeTopAtPolder_Description))]
        public RoundedDouble PhreaticLineOffsetBelowDikeTopAtPolder
        {
            get
            {
                return data.PhreaticLineOffsetBelowDikeTopAtPolder;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PhreaticLineOffsetBelowDikeTopAtPolder = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(phreaticLineOffsetBelowShoulderBaseInsidePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowShoulderBaseInside_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowShoulderBaseInside_Description))]
        public RoundedDouble PhreaticLineOffsetBelowShoulderBaseInside
        {
            get
            {
                return data.PhreaticLineOffsetBelowShoulderBaseInside;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PhreaticLineOffsetBelowShoulderBaseInside = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(phreaticLineOffsetBelowDikeToeAtPolderPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeToeAtPolder_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PhreaticLineOffsetBelowDikeToeAtPolder_Description))]
        public RoundedDouble PhreaticLineOffsetBelowDikeToeAtPolder
        {
            get
            {
                return data.PhreaticLineOffsetBelowDikeToeAtPolder;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PhreaticLineOffsetBelowDikeToeAtPolder = value, propertyChangeHandler);
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return UseDefaultOffsets;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}