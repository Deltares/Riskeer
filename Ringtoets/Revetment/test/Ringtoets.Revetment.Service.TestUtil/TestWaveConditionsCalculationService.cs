﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    /// This class allows for retrieving the output from a calculation, 
    /// so that tests can be performed upon them.
    /// </summary>
    public class TestWaveConditionsCalculationService : IWaveConditionsCalculationService
    {
        public WaveConditionsOutput Calculate(RoundedDouble waterLevel,
                                              double a,
                                              double b,
                                              double c,
                                              double norm,
                                              WaveConditionsInput input,
                                              string hlcdDirectory,
                                              string ringId,
                                              string name)
        {
            return new WaveConditionsOutput(waterLevel, 3.0, 5.39, 29);
        }
    }
}