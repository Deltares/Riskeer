﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Riskeer.Common.Data.Calculation
{
    /// <summary>
    /// Class to allow grouping one or multiple <see cref="ICalculation"/> instances.
    /// </summary>
    public class CalculationGroup : CloneableObservable, ICalculationBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CalculationGroup"/> class.
        /// </summary>
        public CalculationGroup()
        {
            Name = Resources.CalculationGroup_DefaultName;
            Children = new List<ICalculationBase>();
        }

        /// <summary>
        /// Gets the children that define this group.
        /// </summary>
        public List<ICalculationBase> Children { get; private set; }

        public string Name { get; set; }

        public override object Clone()
        {
            var clone = (CalculationGroup) base.Clone();

            clone.Children = Children.Select(c => (ICalculationBase) c.Clone()).ToList();

            return clone;
        }
    }
}