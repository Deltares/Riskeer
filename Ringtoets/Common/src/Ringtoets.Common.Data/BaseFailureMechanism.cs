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
using Core.Common.Base;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// This class is the base implementation for a failure mechanism. Classes which want
    /// to implement IFailureMechanism can and should most likely inherit from this class.
    /// </summary>
    public abstract class BaseFailureMechanism : Observable, IFailureMechanism
    {
        private double contribution;

        public double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (value <= 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100);
                }
                contribution = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the failure mechanism.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        public long StorageId { get; set; }
    }
}