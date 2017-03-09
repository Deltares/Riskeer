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

using System.Xml;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.Revetment.IO;

namespace Ringtoets.StabilityStoneCover.IO
{
    /// <summary>
    /// A writer for writing out configurations of <see cref="StabilityStoneCoverWaveConditionsCalculation"/> and
    /// <see cref="CalculationGroup"/>, to XML format.
    /// </summary>
    public class StabilityStoneCoverConfigurationWriter : WaveConditionsInputConfigurationWriter<StabilityStoneCoverWaveConditionsCalculation>
    {
        protected override void WriteCalculation(StabilityStoneCoverWaveConditionsCalculation calculation, XmlWriter writer)
        {
            WriteCalculation(calculation.Name, calculation.InputParameters, writer);
        }
    }
}