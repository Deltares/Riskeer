// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Ringtoets.Revetment.Data.TestUtil;

namespace Ringtoets.StabilityStoneCover.Data.TestUtil
{
    /// <summary>
    /// Class responsible for generating test data configurations.
    /// </summary>
    public static class StabilityStoneCoverTestDataGenerator
    {
        /// <summary>
        /// Creates a random instance of <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.</returns>
        public static StabilityStoneCoverWaveConditionsCalculation GetRandomStabilityStoneCoverWaveConditionsCalculation()
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Comments =
                {
                    Body = "Random body"
                },
                Name = "Random name"
            };

            WaveConditionsTestDataGenerator.SetRandomDataToWaveConditionsInput(calculation.InputParameters);

            return calculation;
        }

        /// <summary>
        /// Creates a random instance of <see cref="StabilityStoneCoverWaveConditionsOutput"/>.
        /// </summary>
        /// <returns>A random instance of <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        public static StabilityStoneCoverWaveConditionsOutput GetRandomStabilityStoneCoverWaveConditionsOutput()
        {
            return new StabilityStoneCoverWaveConditionsOutput(new[]
                                                               {
                                                                   WaveConditionsTestDataGenerator.GetRandomWaveConditionsOutput()
                                                               },
                                                               new[]
                                                               {
                                                                   WaveConditionsTestDataGenerator.GetRandomWaveConditionsOutput()
                                                               });
        }
    }
}