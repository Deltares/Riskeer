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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IUseForeshore"/>.
    /// </summary>
    public class ConfirmingUseForeshoreProperties<TCalculation, TCalculationInput>
        where TCalculation : ICalculation
        where TCalculationInput : ICalculationInput, IUseForeshore
    {
        private const int useForeshorePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;
        private readonly TCalculationInput data;
        private readonly ICalculationInputPropertyChangeHandler<TCalculationInput, TCalculation> changeHandler;
        private readonly TCalculation calculation;

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingUseForeshoreProperties{TCalculation,TCalculationInput}"/>, in which
        /// all the properties are read only.
        /// </summary>
        public ConfirmingUseForeshoreProperties() { }

        /// <summary>
        /// Creates a new instance of <see cref="ConfirmingUseForeshoreProperties{TCalculation,TCalculationInput}"/>.
        /// </summary>
        /// <param name="useForeshoreData">The data to use for the properties. </param>
        /// <param name="calculation">The calculation to which the <paramref name="useForeshoreData"/> belongs.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public ConfirmingUseForeshoreProperties(
            TCalculationInput useForeshoreData,
            TCalculation calculation,
            ICalculationInputPropertyChangeHandler<TCalculationInput, TCalculation> handler)
        {
            if (useForeshoreData == null)
            {
                throw new ArgumentNullException(nameof(useForeshoreData));
            }
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            data = useForeshoreData;
            this.calculation = calculation;
            changeHandler = handler;
        }

        [DynamicReadOnly]
        [PropertyOrder(useForeshorePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Foreshore_UseForeshore_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Foreshore_UseForeshore_Description))]
        public bool UseForeshore
        {
            get
            {
                return data != null && data.UseForeshore;
            }
            set
            {
                IEnumerable<IObservable> affectedObjects = changeHandler.SetPropertyValueAfterConfirmation(
                    data, 
                    calculation,
                    value, 
                    (input, d) => data.UseForeshore = d);
                NotifyAffectedObjects(affectedObjects);
            }
        }

        [PropertyOrder(coordinatesPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Geometry_Coordinates_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Geometry_Coordinates_Description))]
        public Point2D[] Coordinates
        {
            get
            {
                return data?.ForeshoreGeometry?.ToArray();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data?.ForeshoreGeometry == null || data.ForeshoreGeometry.Count() < 2;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        private static void NotifyAffectedObjects(IEnumerable<IObservable> affectedObjects)
        {
            foreach (var affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }
    }
}