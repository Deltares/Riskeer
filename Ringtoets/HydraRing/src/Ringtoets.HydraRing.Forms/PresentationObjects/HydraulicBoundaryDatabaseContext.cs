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
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Ringtoets.HydraRing.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseContext : Observable
    {
        private readonly HydraulicBoundaryDatabase hydraulicBoundaryDatabase;
        private readonly AssessmentSectionBase baseNode;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseContext"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryDatabase"/> instance wrapped by this context object.</param>
        /// <param name="baseNode">The <see cref="AssessmentSectionBase"/> which the <see cref="HydraulicBoundaryDatabaseContext"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseNode"/> is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseContext(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, AssessmentSectionBase baseNode)
        {
            if (baseNode == null)
            {
                throw new ArgumentNullException("baseNode", "Assessment section cannot be null.");
            }

            this.hydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            this.baseNode = baseNode;
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryDatabase"/> which is wrapped by this context object.
        /// </summary>
        public HydraulicBoundaryDatabase BoundaryDatabase
        {
            get
            {
                return hydraulicBoundaryDatabase;
            }
        }

        /// <summary>
        /// Gets the <see cref="AssessmentSectionBase"/> which this context object belongs to.
        /// </summary>
        public AssessmentSectionBase BaseNode
        {
            get
            {
                return baseNode;
            }
        }
    }
}