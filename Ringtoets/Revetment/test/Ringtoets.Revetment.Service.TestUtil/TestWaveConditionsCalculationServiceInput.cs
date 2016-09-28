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

using Core.Common.Base.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service.TestUtil
{
    /// <summary>
    /// Class used to test the input that is send to the <see cref="WaveConditionsCalculationService"/>.
    /// </summary>
    public class TestWaveConditionsCalculationServiceInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestWaveConditionsCalculationServiceInput"/>.
        /// </summary>
        /// <param name="waterLevel">The specified water level.</param>
        /// <param name="a">The specified 'a' parameter.</param>
        /// <param name="b">The specified 'b' parameter.</param>
        /// <param name="c">The specified 'c' parameter.</param>
        /// <param name="norm">The specified norm.</param>
        /// <param name="input">The specified input.</param>
        /// <param name="hlcdDirectory">The specified HLCD directory .</param>
        /// <param name="ringId">The specified ring id.</param>
        /// <param name="name">The specified name.</param>
        public TestWaveConditionsCalculationServiceInput(RoundedDouble waterLevel,
                                                         double a,
                                                         double b,
                                                         double c,
                                                         double norm,
                                                         WaveConditionsInput input,
                                                         string hlcdDirectory,
                                                         string ringId,
                                                         string name)
        {
            WaterLevel = waterLevel;
            A = a;
            B = b;
            C = c;
            Norm = norm;
            WaveConditionsInput = input;
            HlcdDirectory = hlcdDirectory;
            RingId = ringId;
            Name = name;
        }

        /// <summary>
        /// Gets the specified water level.
        /// </summary>
        public RoundedDouble WaterLevel { get; private set; }

        /// <summary>
        /// Gets the specified 'a' parameter.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// Gets the specified 'b' parameter.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets the specified 'c' parameter.
        /// </summary>
        public double C { get; private set; }

        /// <summary>
        /// Gets the specified norm.
        /// </summary>
        public double Norm { get; private set; }

        /// <summary>
        /// Gets the specified input.
        /// </summary>
        public WaveConditionsInput WaveConditionsInput { get; private set; }

        /// <summary>
        /// Gets the specified HLCD directory.
        /// </summary>
        public string HlcdDirectory { get; private set; }

        /// <summary>
        /// Gets the specified ring id.
        /// </summary>
        public string RingId { get; private set; }

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        public string Name { get; private set; }
    }
}