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

using Core.Common.Base.Data;

using Ringtoets.Common.Forms.Helpers;

using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.TypeConverters
{
    /// <summary>
    /// This class is a specialization of <see cref="FailureMechanismSectionResultNoValueRoundedDoubleConverter"/>,
    /// intended to display probabilities instead of just regular values.
    /// </summary>
    public class FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter :
        FailureMechanismSectionResultNoValueRoundedDoubleConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var roundedDoubleValue = (RoundedDouble)value;
            if (destinationType == typeof(string))
            {
                if (!(double.IsNaN(roundedDoubleValue) || double.IsInfinity(roundedDoubleValue)))
                {
                    return ProbabilityFormattingHelper.Format(roundedDoubleValue);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}