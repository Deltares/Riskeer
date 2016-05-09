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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// Property for probabilistic distribution.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class DistributionProperties : ObjectProperties<IDistribution>
    {
        protected IObservable Observerable;

        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_DestributionType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_DestributionType_Description")]
        public abstract string DistributionType { get; }

        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_Mean_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_Mean_Description")]
        public virtual string Mean
        {
            get
            {
                return new RoundedDouble(2, data.Mean).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                if (Observerable == null)
                {
                    throw new ArgumentException();
                }
                data.Mean = new RoundedDouble(data.StandardDeviation.NumberOfDecimalPlaces, double.Parse(value, CultureInfo.InvariantCulture));
                Observerable.NotifyObservers();
            }
        }

        [ResourcesDisplayName(typeof(Resources), "NormalDistribution_StandardDeviation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "NormalDistribution_StandardDeviation_Description")]
        public virtual string StandardDeviation
        {
            get
            {
                return new RoundedDouble(2, data.StandardDeviation).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                if (Observerable == null)
                {
                    throw new ArgumentException();
                }
                data.StandardDeviation = new RoundedDouble(data.StandardDeviation.NumberOfDecimalPlaces, double.Parse(value, CultureInfo.InvariantCulture));
                Observerable.NotifyObservers();
            }
        }

        public override string ToString()
        {
            return data == null ? Resources.NormalDistribution_StandardDeviation_DisplayName :
                       string.Format("{0} ({1} = {2})", Mean, Resources.NormalDistribution_StandardDeviation_DisplayName, StandardDeviation);
        }
    }
}