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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application.Ringtoets.Storage.DbContext
{
    public class PipingSemiProbabilisticOutputEntity
    {
        public long PipingSemiProbabilisticOutputEntityId { get; set; }
        public long PipingCalculationEntityId { get; set; }
        public int Order { get; set; }
        public double? UpliftFactorOfSafety { get; set; }
        public double? UpliftReliability { get; set; }
        public double? UpliftProbability { get; set; }
        public double? HeaveFactorOfSafety { get; set; }
        public double? HeaveReliability { get; set; }
        public double? HeaveProbability { get; set; }
        public double? SellmeijerFactorOfSafety { get; set; }
        public double? SellmeijerReliability { get; set; }
        public double? SellmeijerProbability { get; set; }
        public double? RequiredProbability { get; set; }
        public double? RequiredReliability { get; set; }
        public double? PipingProbability { get; set; }
        public double? PipingReliability { get; set; }
        public double? PipingFactorOfSafety { get; set; }

        public virtual PipingCalculationEntity PipingCalculationEntity { get; set; }
    }
}