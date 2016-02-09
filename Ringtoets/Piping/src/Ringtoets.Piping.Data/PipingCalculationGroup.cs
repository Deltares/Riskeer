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
using System.Linq;

using Core.Common.Base;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class to allow grouping one or multiple <see cref="PipingCalculation"/> instances.
    /// </summary>
    public class PipingCalculationGroup : Observable, IPipingCalculationItem
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroup"/> class
        /// with an editable name.
        /// </summary>
        public PipingCalculationGroup() : this(Resources.PipingCalculationGroup_DefaultName, true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroup"/> class.
        /// </summary>
        /// <param name="newName">The name of the group.</param>
        /// <param name="canEditName">Determines if the name of the group is editable (true) or not.</param>
        public PipingCalculationGroup(string newName, bool canEditName)
        {
            name = newName;
            IsNameEditable = canEditName;
            Children = new List<IPipingCalculationItem>();
        }

        /// <summary>
        /// Gets the children that define this group.
        /// </summary>
        public IList<IPipingCalculationItem> Children { get; private set; }

        /// <summary>
        /// Gets or sets the name of this calculation grouping object.
        /// </summary>
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
                    throw new InvalidOperationException(Resources.PipingCalculationGroup_Setting_readonly_name_error_message);
                }
                name = value;
            }
        }

        public bool HasOutput
        {
            get
            {
                return Children.Any(c => c.HasOutput);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Name"/> is editable or not.
        /// </summary>
        public bool IsNameEditable { get; private set; }
    }
}