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

using Core.Common.Base.Geometry;

namespace Ringtoets.DuneErosion.Data.TestUtil
{
    /// <summary>
    /// Class that creates simple instances of <see cref="DuneLocation"/>, which
    /// can be used during testing.
    /// </summary>
    public class TestDuneLocation : DuneLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestDuneLocation"/>.
        /// </summary>
        public TestDuneLocation(): this(0, 0, 0) {}

        private TestDuneLocation(int coastalAreaId, double offset, double d50)
            : base(0, string.Empty, new Point2D(0.0, 0.0), new ConstructionProperties
            {
                CoastalAreaId = coastalAreaId,
                Offset = offset,
                D50 = d50,
                Orientation = 0
            }) {}

        /// <summary>
        /// Creates a <see cref="TestDuneLocation"/> with desired values 
        /// that are relevant when exporting a <see cref="DuneLocation"/>.
        /// </summary>
        /// <param name="coastalAreaId">The coastal area id.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="d50">The D50.</param>
        /// <returns>A <see cref="TestDuneLocation"/> with values
        /// that are relevant when exporting.</returns>
        public static TestDuneLocation CreateDuneLocationForExport(int coastalAreaId, double offset, double d50)
        {
            return new TestDuneLocation(coastalAreaId, offset, d50);
        }
    }
}