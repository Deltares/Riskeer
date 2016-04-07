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

using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// This class can be used to create <see cref="Project"/> instances which have their properties set and can be used in tests.
    /// </summary>
    public static class RingtoetsProjectHelper
    {
        /// <summary>
        /// Returns a new complete instance of <see cref="Project"/>.
        /// </summary>
        /// <returns>A new complete instance of <see cref="Project"/>.</returns>
        public static Project GetFullTestProject()
        {
            return new Project
            {
                Name = "tempProjectFile",
                Description = "description",
                Items =
                {
                    new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        Name = "assessmentSection",
                        HydraulicBoundaryDatabase = GetHydraulicBoundaryDatabase(),
                        ReferenceLine = GetReferenceLine()
                    }
                }
            };
        }

        private static ReferenceLine GetReferenceLine()
        {
            IEnumerable<Point2D> points = new[]
            {
                new Point2D(2, 3),
                new Point2D(5, 4),
                new Point2D(5, 8),
                new Point2D(-3, 2)
            };

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);
            return referenceLine;
        }

        private static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase()
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "/temp/test",
                Version = "1.0"
            };
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(13001, "test", 152.3, 2938.5)
            {
                DesignWaterLevel = 12.4
            });

            return hydraulicBoundaryDatabase;
        }
    }
}