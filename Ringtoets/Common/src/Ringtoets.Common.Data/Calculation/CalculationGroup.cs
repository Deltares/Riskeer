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
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Calculation
{
    /// <summary>
    /// Class to allow grouping one or multiple <see cref="ICalculation"/> instances.
    /// </summary>
    public class CalculationGroup : Observable, ICalculationBase
    {
        private string name;

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationGroup"/> class
        /// with an editable name.
        /// </summary>
        public CalculationGroup() : this(Resources.CalculationGroup_DefaultName, true) {}

        /// <summary>
        /// Creates a new instance of the <see cref="CalculationGroup"/> class.
        /// </summary>
        /// <param name="newName">The name of the group.</param>
        /// <param name="canEditName">Determines if the name of the group is editable (<c>true</c>) or not.</param>
        public CalculationGroup(string newName, bool canEditName)
        {
            name = newName;
            IsNameEditable = canEditName;
            Children = new List<ICalculationBase>();
        }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="ICalculationBase.Name"/> is editable.
        /// </summary>
        public bool IsNameEditable { get; }

        /// <summary>
        /// Gets the children that define this group.
        /// </summary>
        public IList<ICalculationBase> Children { get; }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!IsNameEditable)
                {
                    throw new InvalidOperationException(Resources.CalculationGroup_Setting_readonly_name_error_message);
                }
                name = value;
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}