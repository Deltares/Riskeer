// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riskeer.Storage.Core.DbContext
{
    public class DuneLocationCalculationOutputEntity
    {
        public long DuneLocationCalculationOutputEntityId { get; set; }
        public long DuneLocationCalculationEntityId { get; set; }
        public double? WaterLevel { get; set; }
        public double? WaveHeight { get; set; }
        public double? WavePeriod { get; set; }
        public double? MeanTidalAmplitude { get; set; }
        public double? WaveDirectionalSpread { get; set; }
        public double? TideSurgePhaseDifference { get; set; }
        public double? TargetProbability { get; set; }
        public double? TargetReliability { get; set; }
        public double? CalculatedProbability { get; set; }
        public double? CalculatedReliability { get; set; }
        public byte CalculationConvergence { get; set; }

        public virtual DuneLocationCalculationEntity DuneLocationCalculationEntity { get; set; }
    }
}