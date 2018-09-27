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
using Core.Common.Util.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsLocationInputBase"/> for properties panel.
    /// </summary>
    /// <typeparam name="T">The type of location input.</typeparam>
    public abstract class MacroStabilityInwardsLocationInputBaseProperties<T> : ObjectProperties<T> where T : MacroStabilityInwardsLocationInputBase
    {
        private const int waterLevelPolderPropertyIndex = 1;
        private const int offsetsPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsLocationInputBaseProperties{T}"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected MacroStabilityInwardsLocationInputBaseProperties(T data, IObservablePropertyChangeHandler handler)
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
            PropertyChangeHandler = handler;
        }

        [PropertyOrder(waterLevelPolderPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaterLevelPolder_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaterLevelPolder_Description))]
        public RoundedDouble WaterLevelPolder
        {
            get
            {
                return data.WaterLevelPolder;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WaterLevelPolder = value, PropertyChangeHandler);
            }
        }

        [PropertyOrder(offsetsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Offsets_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Offsets_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsLocationInputOffsetProperties Offsets
        {
            get
            {
                return new MacroStabilityInwardsLocationInputOffsetProperties(data, PropertyChangeHandler);
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the handler responsible for handling effects of a property change.
        /// </summary>
        protected IObservablePropertyChangeHandler PropertyChangeHandler { get; }
    }
}