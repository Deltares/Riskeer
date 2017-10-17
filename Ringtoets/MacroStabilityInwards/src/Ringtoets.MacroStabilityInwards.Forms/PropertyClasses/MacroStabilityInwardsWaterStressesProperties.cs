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
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Properties;

namespace Ringtoets.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsInputContext"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsWaterStressesProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int waterLevelRiverAveragePropertyIndex = 1;
        private const int drainagePropertyIndex = 2;
        private const int minimumLevelPhreaticLineAtDikeTopRiverPropertyIndex = 3;
        private const int minimumLevelPhreaticLineAtDikeTopPolderPropertyIndex = 4;
        private const int adjustPhreaticLine3And4ForUpliftPropertyIndex = 5;
        private const int leakageLengthOutwardsPhreaticLine3PropertyIndex = 6;
        private const int leakageLengthInwardsPhreaticLine3PropertyIndex = 7;
        private const int leakageLengthOutwardsPhreaticLine4PropertyIndex = 8;
        private const int leakageLengthInwardsPhreaticLine4PropertyIndex = 9;
        private const int piezometricHeadPhreaticLine2OutwardsPropertyIndex = 10;
        private const int piezometricHeadPhreaticLine2InwardsPropertyIndex = 11;
        private const int locationExtremePropertyIndex = 12;
        private const int locationDailyPropertyIndex = 13;
        private const int waterStressLinesPropertyIndex = 14;

        private readonly IObservablePropertyChangeHandler propertyChangeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaterStressesProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="handler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsWaterStressesProperties(MacroStabilityInwardsInput data, IObservablePropertyChangeHandler handler)
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

        [PropertyOrder(waterLevelRiverAveragePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaterLevelRiverAverage_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaterLevelRiverAverage_Description))]
        public RoundedDouble WaterLevelRiverAverage
        {
            get
            {
                return data.WaterLevelRiverAverage;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.WaterLevelRiverAverage = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(drainagePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DrainageConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DrainageConstruction_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsDrainageProperties Drainage
        {
            get
            {
                return new MacroStabilityInwardsDrainageProperties(data, propertyChangeHandler);
            }
        }

        [PropertyOrder(minimumLevelPhreaticLineAtDikeTopRiverPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MinimumLevelPhreaticLineAtDikeTopRiver_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MinimumLevelPhreaticLineAtDikeTopRiver_Description))]
        public RoundedDouble MinimumLevelPhreaticLineAtDikeTopRiver
        {
            get
            {
                return data.MinimumLevelPhreaticLineAtDikeTopRiver;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.MinimumLevelPhreaticLineAtDikeTopRiver = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(minimumLevelPhreaticLineAtDikeTopPolderPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MinimumLevelPhreaticLineAtDikeTopPolder_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MinimumLevelPhreaticLineAtDikeTopPolder_Description))]
        public RoundedDouble MinimumLevelPhreaticLineAtDikeTopPolder
        {
            get
            {
                return data.MinimumLevelPhreaticLineAtDikeTopPolder;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.MinimumLevelPhreaticLineAtDikeTopPolder = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(adjustPhreaticLine3And4ForUpliftPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AdjustPhreaticLine3And4ForUplift_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.AdjustPhreaticLine3And4ForUplift_Description))]
        public bool AdjustPhreaticLine3And4ForUplift
        {
            get
            {
                return data.AdjustPhreaticLine3And4ForUplift;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.AdjustPhreaticLine3And4ForUplift = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(leakageLengthOutwardsPhreaticLine3PropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeakageLengthOutwardsPhreaticLine3_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeakageLengthOutwardsPhreaticLine3_Description))]
        public RoundedDouble LeakageLengthOutwardsPhreaticLine3
        {
            get
            {
                return data.LeakageLengthOutwardsPhreaticLine3;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LeakageLengthOutwardsPhreaticLine3 = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(leakageLengthInwardsPhreaticLine3PropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeakageLengthInwardsPhreaticLine3_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeakageLengthInwardsPhreaticLine3_Description))]
        public RoundedDouble LeakageLengthInwardsPhreaticLine3
        {
            get
            {
                return data.LeakageLengthInwardsPhreaticLine3;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LeakageLengthInwardsPhreaticLine3 = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(leakageLengthOutwardsPhreaticLine4PropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeakageLengthOutwardsPhreaticLine4_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeakageLengthOutwardsPhreaticLine4_Description))]
        public RoundedDouble LeakageLengthOutwardsPhreaticLine4
        {
            get
            {
                return data.LeakageLengthOutwardsPhreaticLine4;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LeakageLengthOutwardsPhreaticLine4 = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(leakageLengthInwardsPhreaticLine4PropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LeakageLengthInwardsPhreaticLine4_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.LeakageLengthInwardsPhreaticLine4_Description))]
        public RoundedDouble LeakageLengthInwardsPhreaticLine4
        {
            get
            {
                return data.LeakageLengthInwardsPhreaticLine4;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.LeakageLengthInwardsPhreaticLine4 = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(piezometricHeadPhreaticLine2OutwardsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PiezometricHeadPhreaticLine2Outwards_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PiezometricHeadPhreaticLine2Outwards_Description))]
        public RoundedDouble PiezometricHeadPhreaticLine2Outwards
        {
            get
            {
                return data.PiezometricHeadPhreaticLine2Outwards;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PiezometricHeadPhreaticLine2Outwards = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(piezometricHeadPhreaticLine2InwardsPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.PiezometricHeadPhreaticLine2Inwards_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.PiezometricHeadPhreaticLine2Inwards_Description))]
        public RoundedDouble PiezometricHeadPhreaticLine2Inwards
        {
            get
            {
                return data.PiezometricHeadPhreaticLine2Inwards;
            }
            set
            {
                PropertyChangeHelper.ChangePropertyAndNotify(() => data.PiezometricHeadPhreaticLine2Inwards = value, propertyChangeHandler);
            }
        }

        [PropertyOrder(locationExtremePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LocationExtreme_DisplayName))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsLocationInputExtremeProperties LocationExtreme
        {
            get
            {
                return new MacroStabilityInwardsLocationInputExtremeProperties((MacroStabilityInwardsLocationInputExtreme) data.LocationInputExtreme, propertyChangeHandler);
            }
        }

        [PropertyOrder(locationDailyPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.LocationDaily_DisplayName))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsLocationInputDailyProperties LocationDaily
        {
            get
            {
                return new MacroStabilityInwardsLocationInputDailyProperties((MacroStabilityInwardsLocationInputDaily) data.LocationInputDaily, propertyChangeHandler);
            }
        }

        [PropertyOrder(waterStressLinesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaterStressLines_DisplayName))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsWaterStressLinesProperties WaterStressLines
        {
            get
            {
                return new MacroStabilityInwardsWaterStressLinesProperties(data);
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}