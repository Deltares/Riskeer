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

using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Integration.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseContext : Observable
    {
        private readonly IAssessmentSection parent;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseContext"/>.
        /// </summary>
        /// <param name="parent">The <see cref="IAssessmentSection"/> which the <see cref="HydraulicBoundaryDatabaseContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parent"/> is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseContext(IAssessmentSection parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent", "Assessment section cannot be null.");
            }

            this.parent = parent;
        }

        /// <summary>
        /// Gets the <see cref="IAssessmentSection"/> which this context object belongs to.
        /// </summary>
        public IAssessmentSection Parent
        {
            get
            {
                return parent;
            }
        }

        #region Equal implementation

        private bool Equals(HydraulicBoundaryDatabaseContext other)
        {
            return Equals(parent, other.parent);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((HydraulicBoundaryDatabaseContext) obj);
        }

        public override int GetHashCode()
        {
            return (parent != null ? parent.GetHashCode() : 0);
        }

        #endregion
    }
}